using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PurrNet.Editor
{
    [CustomPropertyDrawer(typeof(SyncVar<>), true)]
    public class SyncVarDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, bool> _valueFoldoutStates = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded, label, true);

            if (!property.isExpanded)
                return;

            EditorGUI.indentLevel++;
            float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var copy = property.Copy();
            var end = copy.GetEndProperty();

            if (copy.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(copy, end)) break;

                    if (copy.name == "_value")
                    {
                        string foldoutKey = property.propertyPath + "._value";
                        if (!_valueFoldoutStates.TryGetValue(foldoutKey, out bool isExpanded))
                            isExpanded = false;

                        Rect foldoutRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                        bool newExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, "Value", true);
                        _valueFoldoutStates[foldoutKey] = newExpanded;
                        y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        if (!newExpanded)
                            continue;

                        // Lock check for the parent _value field, not children
                        bool isLocked = Application.isPlaying && FieldIsLocked(copy);
                        if (isLocked) EditorGUI.BeginDisabledGroup(true);

                        var valueCopy = copy.Copy();
                        var valueEnd = valueCopy.GetEndProperty();

                        if (valueCopy.NextVisible(true) && !SerializedProperty.EqualContents(valueCopy, valueEnd))
                        {
                            int indent = EditorGUI.indentLevel + 1;
                            EditorGUI.indentLevel = indent;

                            do
                            {
                                if (SerializedProperty.EqualContents(valueCopy, valueEnd))
                                    break;

                                float h = EditorGUI.GetPropertyHeight(valueCopy, true);
                                EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), valueCopy, true);
                                y += h + EditorGUIUtility.standardVerticalSpacing;
                            }
                            while (valueCopy.NextVisible(false));

                            EditorGUI.indentLevel = indent - 1;
                        }
                        else
                        {
                            float h = EditorGUI.GetPropertyHeight(copy, true);
                            EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), copy, true);
                            y += h + EditorGUIUtility.standardVerticalSpacing;
                        }

                        if (isLocked) EditorGUI.EndDisabledGroup();

                        continue;
                    }

                    float height = EditorGUI.GetPropertyHeight(copy, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), copy, true);
                    y += height + EditorGUIUtility.standardVerticalSpacing;
                }
                while (copy.NextVisible(false));
            }

            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded)
                return height;

            var copy = property.Copy();
            var end = copy.GetEndProperty();

            if (copy.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(copy, end)) break;

                    if (copy.name == "_value")
                    {
                        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        string foldoutKey = property.propertyPath + "._value";
                        if (!_valueFoldoutStates.TryGetValue(foldoutKey, out bool isExpanded))
                            isExpanded = false;

                        if (!isExpanded)
                            continue;

                        var valueCopy = copy.Copy();
                        var valueEnd = valueCopy.GetEndProperty();

                        if (valueCopy.NextVisible(true) && !SerializedProperty.EqualContents(valueCopy, valueEnd))
                        {
                            do
                            {
                                if (SerializedProperty.EqualContents(valueCopy, valueEnd))
                                    break;

                                height += EditorGUI.GetPropertyHeight(valueCopy, true) + EditorGUIUtility.standardVerticalSpacing;
                            }
                            while (valueCopy.NextVisible(false));
                        }
                        else
                        {
                            height += EditorGUI.GetPropertyHeight(copy, true) + EditorGUIUtility.standardVerticalSpacing;
                        }

                        continue;
                    }

                    height += EditorGUI.GetPropertyHeight(copy, true) + EditorGUIUtility.standardVerticalSpacing;
                }
                while (copy.NextVisible(false));
            }

            return height;
        }
        
        private bool FieldIsLocked(SerializedProperty property)
        {
            return true;
        }
    }
}
