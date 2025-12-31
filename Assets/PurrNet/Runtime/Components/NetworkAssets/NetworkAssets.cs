using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PurrNet
{
    [CreateAssetMenu(fileName = "NetworkAssets", menuName = "PurrNet/Network Assets", order = -200)]
    public class NetworkAssets : ScriptableObject
    {
        public bool autoGenerate;
        public Object folder;

        [Serializable]
        public class TypeToggle
        {
            public string typeName;
            public bool enabled;
        }

        [SerializeField] private List<string> _enabledTypeNames = new();
        private HashSet<string> _enabledTypeLookup;

        public HashSet<string> enabledTypeNames
        {
            get
            {
                return _enabledTypeLookup ??= new HashSet<string>(_enabledTypeNames);
            }
        }

        public List<Object> assets = new();

        [SerializeField, HideInInspector]
        private List<string> _availableTypeNames = new();
        public IReadOnlyList<string> AvailableTypeNames => _availableTypeNames;

        private readonly Dictionary<int, Object> idToAsset = new();
        private readonly Dictionary<Object, int> assetToId = new();

        [SerializeField, HideInInspector] private List<int> _bakedIds = new();
        [SerializeField, HideInInspector] private List<Object> _bakedAssets = new();

        public Object GetAsset(int index) => idToAsset.GetValueOrDefault(index);

        public int GetIndex(Object obj)
        {
            return assetToId.GetValueOrDefault(obj, -1);
        }

        private void OnEnable()
        {
            idToAsset.Clear();
            assetToId.Clear();

            for (int i = 0; i < _bakedAssets.Count; i++)
            {
                var obj = _bakedAssets[i];
                int id = _bakedIds[i];
                if (!obj) continue;

                try
                {
                    idToAsset[id] = obj;
                    assetToId[obj] = id;
                }
                catch
                {
                    idToAsset.Remove(id);
                }
            }
        }

        public IReadOnlyList<Object> AllAssets => assets;
        public IReadOnlyDictionary<int, Object> IndexToAsset => idToAsset;
        public IReadOnlyDictionary<Object, int> AssetToIndex => assetToId;

        public void Refresh()
        {
            idToAsset.Clear();
            assetToId.Clear();
            _bakedIds.Clear();
            _bakedAssets.Clear();

            for (int i = 0; i < assets.Count; i++)
            {
                var obj = assets[i];
                if (!obj || assetToId.ContainsKey(obj)) continue;

                idToAsset[i] = obj;
                assetToId[obj] = i;

                _bakedIds.Add(i);
                _bakedAssets.Add(obj);
            }
        }

        public void AddAsset(Object obj, bool logIfDuplicate = true)
        {
            if (!obj) return;

            if (assetToId.TryGetValue(obj, out _))
            {
                if (logIfDuplicate)
                    Debug.LogWarning($"Asset already exists in NetworkAssets: {obj.name}");
                return;
            }

            assets.Add(obj);
            Refresh();
        }

        public void CacheAvailableTypes(IEnumerable<Type> types)
        {
            _availableTypeNames = types.Select(t => t.AssemblyQualifiedName).Distinct().ToList();
        }

        public void SetEnabledType(string typeName, bool enable)
        {
            if (enable)
            {
                if (!_enabledTypeNames.Contains(typeName))
                    _enabledTypeNames.Add(typeName);
            }
            else
            {
                _enabledTypeNames.Remove(typeName);
            }

            _enabledTypeLookup = null;
        }
        
#if UNITY_EDITOR
        public void GenerateAssets()
        {
            if (!folder) return;

            var enabledTypes = enabledTypeNames.Select(System.Type.GetType).Where(t => t != null).ToArray();
            string path = UnityEditor.AssetDatabase.GetAssetPath(folder);
            string[] guids = UnityEditor.AssetDatabase.FindAssets("", new[] { path });

            var found = new HashSet<UnityEngine.Object>();
            foreach (var guid in guids)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.EndsWith(".unity")) continue;

                var all = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (var obj in all)
                {
                    if (!obj || obj.GetType().Namespace?.Contains("UnityEditor") == true) continue;
                    if (enabledTypes.Any(t => t.IsAssignableFrom(obj.GetType())) && !assets.Contains(obj))
                        found.Add(obj);
                }
            }

            foreach (var obj in found)
                assets.Add(obj);

            if (found.Count > 0)
            {
                Refresh();
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
            }

            CleanupNullEntries();
        }

        private void CleanupNullEntries()
        {
            assets.RemoveAll(a => a == null);
            Refresh();
        }
#endif

        public bool TryGetAsset(int id, out Object obj) => idToAsset.TryGetValue(id, out obj);
        public bool TryGetId(Object obj, out int id) => assetToId.TryGetValue(obj, out id);
    }

}
