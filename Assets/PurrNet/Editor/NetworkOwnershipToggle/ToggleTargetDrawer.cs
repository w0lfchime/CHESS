using UnityEditor;
using UnityEngine;

namespace PurrNet.Editor
{
    [CustomPropertyDrawer(typeof(OwnershipComponentToggle))]
    [CustomPropertyDrawer(typeof(OwnershipGameObjectToggle))]
    public class ToggleTargetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.IndentedRect(position);

            var targetProperty = property.FindPropertyRelative("target");
            var activeAsOwnerProperty = property.FindPropertyRelative("activeAsOwner");

            float toggleWidth = EditorGUI.GetPropertyHeight(activeAsOwnerProperty);
            float spacing = 5f;

            Rect targetRect = new Rect(position.x, position.y, position.width - toggleWidth - spacing, position.height);
            Rect stateRect = new Rect(targetRect.xMax + spacing, position.y, toggleWidth, position.height);

            // Draw fields
            EditorGUI.PropertyField(targetRect, targetProperty, GUIContent.none);
            activeAsOwnerProperty.boolValue = EditorGUI.Toggle(stateRect, activeAsOwnerProperty.boolValue);

            EditorGUI.EndProperty();
        }
    }
}
