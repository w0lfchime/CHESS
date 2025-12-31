using UnityEngine;

namespace PurrNet
{
    public struct QuaternionWithParent
    {
        readonly Transform parent;
        public readonly Quaternion rotation;
        readonly bool isLocalPos;

        public QuaternionWithParent(Transform parent, bool isLocalPos, Quaternion rotation)
        {
            this.parent = parent;
            this.rotation = rotation;
            this.isLocalPos = isLocalPos;
        }

        public static QuaternionWithParent Lerp(QuaternionWithParent a, QuaternionWithParent b, float t)
        {
            if (!b.isLocalPos)
                return new QuaternionWithParent(default, default, Quaternion.Lerp(a.rotation, b.rotation, t));

            var aWorldRot = a.parent ? a.parent.rotation * a.rotation : a.rotation;
            var bWorldRot = b.parent ? b.parent.rotation * b.rotation : b.rotation;

            var lerpedWorldRot = Quaternion.Lerp(aWorldRot, bWorldRot, t);
            return new QuaternionWithParent(default, default, lerpedWorldRot);
        }

        public static QuaternionWithParent NoLerp(QuaternionWithParent a, QuaternionWithParent b, float t)
        {
            if (!b.isLocalPos)
                return new QuaternionWithParent(default, default, b.rotation);

            var bWorldRot = b.parent ? b.parent.rotation * b.rotation : b.rotation;
            return new QuaternionWithParent(default, default, bWorldRot);
        }
    }
}
