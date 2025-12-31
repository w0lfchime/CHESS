using UnityEngine;

namespace PurrNet
{
    public struct ScaleWithParent
    {
        readonly Transform parent;
        public readonly Vector3 scale;

        public ScaleWithParent(Transform parent, Vector3 scale)
        {
            this.parent = parent;
            this.scale = scale;
        }

        public static ScaleWithParent Lerp(ScaleWithParent a, ScaleWithParent b, float t)
        {
            var aWorldScale = a.parent ? a.parent.GetWorldScale(a.scale) : a.scale;
            var bWorldScale = b.parent ? b.parent.GetWorldScale(b.scale) : b.scale;

            return new ScaleWithParent(null, Vector3.Lerp(aWorldScale, bWorldScale, t));
        }

        public static ScaleWithParent NoLerp(ScaleWithParent a, ScaleWithParent b, float t)
        {
            if (!b.parent)
                return new ScaleWithParent(default, b.scale);
            var worldScale = b.parent ? b.parent.GetWorldScale(b.scale) : b.scale;
            return new ScaleWithParent(default, worldScale);
        }
    }
}
