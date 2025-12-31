using System;
using JetBrains.Annotations;
#if UNITY_EDITOR && PURR_BUTTONS
using System.Collections.Generic;
using System.Reflection;
using PurrNet.Logging;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Scripting;

namespace PurrNet
{
    [AttributeUsage(AttributeTargets.Method), UsedImplicitly]
    public class PurrContextButtonAttribute
#if PURR_CONTEXT_BUTTONS
        : PurrButtonAttribute { }
#else
        : Attribute {}
#endif

    [AttributeUsage(AttributeTargets.Method), UsedImplicitly]
    public class PurrButtonAttribute : PreserveAttribute
    {
        public string ButtonName { get; private set; }

        public PurrButtonAttribute(string buttonName = "")
        {
            ButtonName = buttonName;
        }
    }

#if UNITY_EDITOR && PURR_BUTTONS
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class PurrButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MonoBehaviour targetMB = (MonoBehaviour)target;

            var allMethods = new List<MethodInfo>();
            var current = target.GetType();

            while (current != null && current != typeof(MonoBehaviour))
            {
                allMethods.AddRange(current.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public));
                current = current.BaseType;
            }

            foreach (var method in allMethods)
            {
                var attr = method.GetCustomAttribute<PurrButtonAttribute>();
                if (attr == null) continue;

                var buttonName = !string.IsNullOrEmpty(attr.ButtonName) ? attr.ButtonName : ObjectNames.NicifyVariableName(method.Name);

                if (GUILayout.Button(buttonName))
                {
                    if (method.GetParameters().Length == 0)
                        method.Invoke(target, null);
                    else
                        PurrLogger.LogWarning($"Cannot invoke method '{method.Name}' with PurrButton because it has parameters.");
                }
            }
        }
    }
#endif
}
