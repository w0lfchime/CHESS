#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PurrNet
{
    [CustomEditor(typeof(NetworkAssets))]
    public class NetworkAssetsEditor : UnityEditor.Editor
    {
        private NetworkAssets _target;
        private SerializedProperty _folderProp;
        [UsedImplicitly]
        private SerializedProperty _autoGenerateProp;
        private SerializedProperty _assetsProp;

        private bool _showTypeList;
        private string _typeSearch = "";
        private List<Type> _cachedTypes;
        private int _typePage = 0;
        private const int TypesPerPage = 10;

        [UsedImplicitly]
        private const int AssetsPerPage = 20;
        [UsedImplicitly]
        private int _assetPage;

        private static GUIStyle DescriptionStyle()
        {
            return new GUIStyle(GUI.skin.label)
            {
                wordWrap = true
            };
        }

        private void OnEnable()
        {
            _cachedTypes = null;
            _target = (NetworkAssets)target;
            _folderProp = serializedObject.FindProperty("folder");
            _autoGenerateProp = serializedObject.FindProperty("autoGenerate");
            _assetsProp = serializedObject.FindProperty("assets");

            _cachedTypes = _target.AvailableTypeNames
                .Select(Type.GetType)
                .Where(t => t != null)
                .OrderByDescending(t => _target.enabledTypeNames.Contains(t.AssemblyQualifiedName))
                .ThenBy(t => t.Name)
                .ToList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.Label("Network Assets", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "This asset is used to store Unity objects (e.g., Materials, Sprites, Scriptables) to reference them by index for efficient networking.",
                DescriptionStyle());
            GUILayout.Space(10);

            EditorGUILayout.PropertyField(_folderProp);

            GUILayout.BeginHorizontal();

            DrawToggleButton("Auto generate", ref _target.autoGenerate);
            if (GUILayout.Button("Generate", GUILayout.Width(1), GUILayout.ExpandWidth(true)))
            {
                Generate();
            }

            GUILayout.EndHorizontal();


            if (GUILayout.Button("Refresh Type List"))
            {
                _cachedTypes = GetTypesWithAssetsSorted(_target);
            }

            DrawTypeToggleFoldout();
            GUILayout.Space(10);

            DrawAssetList();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                _target.Refresh();
                EditorUtility.SetDirty(_target);
            }
        }

        private void DrawToggleButton(string label, ref bool value)
        {
            GUI.color = value ? Color.green : Color.white;
            if (GUILayout.Button(label, GUILayout.Width(1), GUILayout.ExpandWidth(true)))
            {
                value = !value;
                EditorUtility.SetDirty(_target);
            }
            GUI.color = Color.white;
        }

        private void DrawAssetList()
        {
            EditorGUILayout.PropertyField(_assetsProp, true);
        }

        private void DrawTypeToggleFoldout()
        {
            try
            {
                EditorGUILayout.BeginVertical("box");
                _showTypeList = EditorGUILayout.Foldout(_showTypeList, "Filter Types", true);
                if (!_showTypeList)
                {
                    EditorGUILayout.EndVertical();
                    return;
                }

                _typeSearch = EditorGUILayout.TextField("Search", _typeSearch);

                if (_cachedTypes == null)
                {
                    if (_target.AvailableTypeNames != null && _target.AvailableTypeNames.Count > 0)
                    {
                        _cachedTypes = _target.AvailableTypeNames
                            .Select(Type.GetType)
                            .Where(t => t != null)
                            .ToList();
                    }
                    else
                    {
                        _cachedTypes = GetTypesWithAssetsSorted(_target);
                    }
                }

                var filtered = string.IsNullOrEmpty(_typeSearch)
                    ? _cachedTypes
                    : _cachedTypes.Where(t => t.Name.IndexOf(_typeSearch, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                int totalPages = Mathf.CeilToInt(filtered.Count / (float)TypesPerPage);
                _typePage = Mathf.Clamp(_typePage, 0, Mathf.Max(0, totalPages - 1));

                int start = _typePage * TypesPerPage;
                int end = Mathf.Min(start + TypesPerPage, filtered.Count);

                for (int i = start; i < end; i++)
                {
                    var type = filtered[i];
                    string typeName = type.AssemblyQualifiedName;
                    bool enabled = _target.enabledTypeNames.Contains(typeName);
                    bool newValue = EditorGUILayout.ToggleLeft(type.Name, enabled);

                    if (newValue != enabled)
                    {
                        _target.SetEnabledType(typeName, newValue);

                        _cachedTypes = _cachedTypes
                            .OrderByDescending(t => _target.enabledTypeNames.Contains(t.AssemblyQualifiedName))
                            .ThenBy(t => t.Name)
                            .ToList();

                        EditorUtility.SetDirty(_target);
                    }
                }

                EditorGUILayout.BeginHorizontal();
                GUI.enabled = _typePage > 0;
                if (GUILayout.Button("Prev")) _typePage--;
                GUI.enabled = _typePage < totalPages - 1;
                if (GUILayout.Button("Next")) _typePage++;
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private void Generate()
        {
            if (!_target.folder) return;

            var enabledTypes = _target.enabledTypeNames
                .Select(Type.GetType)
                .Where(t => t != null)
                .ToArray();

            string path = AssetDatabase.GetAssetPath(_target.folder);
            string[] guids = AssetDatabase.FindAssets("", new[] { path });

            var found = new HashSet<UnityEngine.Object>();
            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (assetPath.EndsWith(".unity"))
                    continue;

                var all = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (var obj in all)
                {
                    if (!obj) continue;

                    var ns = obj.GetType().Namespace;
                    if (ns != null && ns.Contains("UnityEditor"))
                        continue;

                    if (obj && enabledTypes.Any(t => t.IsAssignableFrom(obj.GetType())) && !_target.assets.Contains(obj))
                        _target.assets.Add(obj);
                }
            }

            bool changed = false;

            foreach (var obj in found)
            {
                if (!_target.assets.Contains(obj))
                {
                    _target.assets.Add(obj);
                    changed = true;
                }
            }

            if (changed)
            {
                _target.Refresh();
                EditorUtility.SetDirty(_target);
                AssetDatabase.SaveAssets();
            }

            _target.Refresh();

            EditorUtility.SetDirty(_target);
            AssetDatabase.SaveAssets();
        }

        private static List<Type> GetTypesWithAssetsSorted(NetworkAssets target)
        {
            string[] guids = AssetDatabase.FindAssets("t:Object");
            HashSet<Type> types = new();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path) || path.StartsWith("Assets/") == false || path.EndsWith(".unity"))
                    continue;

                var all = AssetDatabase.LoadAllAssetsAtPath(path);

                foreach (var obj in all)
                {
                    if (!obj) continue;

                    Type type = obj.GetType();
                    while (type != null && typeof(UnityEngine.Object).IsAssignableFrom(type))
                    {
                        types.Add(type);
                        type = type.BaseType;
                    }
                }
            }

            var sorted = types
                .OrderByDescending(t => target.enabledTypeNames.Contains(t.AssemblyQualifiedName))
                .ThenBy(t => t.Name)
                .ToList();

            target.CacheAvailableTypes(sorted);
            EditorUtility.SetDirty(target);

            return sorted;
        }
    }
}
#endif
