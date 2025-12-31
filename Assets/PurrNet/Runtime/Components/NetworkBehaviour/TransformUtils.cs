using UnityEngine;

namespace PurrNet
{
    public static class TransformUtils
    {
        public static Vector3 GetWorldScale(this Transform parent, Vector3 localScale)
        {
            Vector3 worldScale = localScale;
            Transform t = parent;
            while (t != null)
            {
                worldScale = Vector3.Scale(worldScale, t.localScale);
                t = t.parent;
            }
            return worldScale;
        }


        public static Vector3 GetLocalScale(this Transform parent, Vector3 desiredWorldScale)
        {
            Vector3 parentWorldScale = Vector3.one;
            Transform t = parent;
            while (t != null)
            {
                parentWorldScale = Vector3.Scale(parentWorldScale, t.localScale);
                t = t.parent;
            }

            // Avoid division by zero
            return new Vector3(
                desiredWorldScale.x / parentWorldScale.x,
                desiredWorldScale.y / parentWorldScale.y,
                desiredWorldScale.z / parentWorldScale.z
            );
        }
    }
}
