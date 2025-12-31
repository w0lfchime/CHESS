using UnityEngine;
#if !UNITY_6000_0_OR_NEWER
using PurrNet.Pooling;
#endif

public static class ComponentExtensions
{
    #region Animator

#if !UNITY_2022_2_OR_NEWER

    private static readonly System.Reflection.FieldInfo _avatarRootField;

    static ComponentExtensions()
    {
        _avatarRootField = typeof(Animator).GetField("avatarRoot",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    }
#endif

    public static Transform GetAvatarRoot(this Animator animator)
    {
#if !UNITY_2022_2_OR_NEWER
        if (animator == null || _avatarRootField == null)
            return null;
        return _avatarRootField.GetValue(animator) as Transform;
#else
        return animator ? animator.avatarRoot : null;
#endif
    }

    #endregion

#if !UNITY_6000_0_OR_NEWER
    public static int GetComponentIndex(this Component component)
    {
        if (!component)
            return -1;

        using var components = DisposableList<Component>.Create(16);
        component.gameObject.GetComponents<Component>(components.list);

        for (int i = 0; i < components.Count; i++)
        {
            if (components[i] == component)
                return i;
        }

        return -1;
    }

    public static T GetComponentAtIndex<T>(this GameObject gameObject, int index) where T : Component
    {
        if (gameObject == null || index < 0)
            return null;

        using var components = DisposableList<Component>.Create(16);
        gameObject.GetComponents<Component>(components.list);

        if (index >= components.Count)
            return null;

        if (components[index] == null)
            return null;

        if (components[index] is T result)
            return result;

        return null;
    }
#endif
}
