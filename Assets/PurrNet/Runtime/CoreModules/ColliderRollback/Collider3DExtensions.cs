#if UNITY_PHYSICS_3D
using UnityEngine;

namespace PurrNet.Modules
{
    public static class Collider3DExtensions
    {
        static readonly RaycastHit[] _raycastHits = new RaycastHit[1024];
        static readonly Collider[] results = new Collider[1024];

        public static bool SphereCast(this Collider collider, Ray ray, float radius, out RaycastHit hitInfo, float maxDistance)
        {
            if (!collider)
            {
                hitInfo = default;
                return false;
            }

            var go = collider.gameObject;
            var scene = go.scene.GetPhysicsScene();
            var myLayer = 1 << go.layer;
            int count = scene.SphereCast(ray.origin, radius, ray.direction, _raycastHits, maxDistance, myLayer, QueryTriggerInteraction.Collide);
            for (int i = 0; i < count; i++)
            {
                if (_raycastHits[i].collider == collider)
                {
                    hitInfo = _raycastHits[i];
                    return true;
                }
            }

            hitInfo = default;
            return false;
        }

        public static bool BoxCast(this Collider collider, Ray ray, Vector3 halfExtents, Quaternion orientation, out RaycastHit hitInfo, float maxDistance)
        {
            if (!collider)
            {
                hitInfo = default;
                return false;
            }

            var go = collider.gameObject;
            var scene = go.scene.GetPhysicsScene();
            var myLayer = 1 << go.layer;
            int count = scene.BoxCast(ray.origin, halfExtents, ray.direction, _raycastHits, orientation, maxDistance, myLayer, QueryTriggerInteraction.Collide);
            for (int i = 0; i < count; i++)
            {
                if (_raycastHits[i].collider == collider)
                {
                    hitInfo = _raycastHits[i];
                    return true;
                }
            }

            hitInfo = default;
            return false;
        }

        public static bool CapsuleCast(this Collider collider, Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
        {
            if (!collider)
            {
                hitInfo = default;
                return false;
            }

            var go = collider.gameObject;
            var scene = go.scene.GetPhysicsScene();
            var myLayer = 1 << go.layer;
            int count = scene.CapsuleCast(point1, point2, radius, direction, _raycastHits, maxDistance, myLayer, QueryTriggerInteraction.Collide);
            for (int i = 0; i < count; i++)
            {
                if (_raycastHits[i].collider == collider)
                {
                    hitInfo = _raycastHits[i];
                    return true;
                }
            }

            hitInfo = default;
            return false;
        }

        public static bool OverlapSphere(this Collider collider, Vector3 position, float radius)
        {
            if (!collider)
                return false;

            var go = collider.gameObject;
            var scene = go.scene.GetPhysicsScene();
            var myLayer = 1 << go.layer;
            int count = scene.OverlapSphere(position, radius, results, myLayer, QueryTriggerInteraction.Collide);

            for (int i = 0; i < count; i++)
            {
                if (results[i] == collider)
                    return true;
            }

            return false;
        }

        public static bool OverlapBox(this Collider collider, Vector3 position, Vector3 halfExtents, Quaternion orientation)
        {
            if (!collider)
                return false;

            var go = collider.gameObject;
            var scene = go.scene.GetPhysicsScene();
            var myLayer = 1 << go.layer;
            int count = scene.OverlapBox(position, halfExtents, results, orientation, myLayer, QueryTriggerInteraction.Collide);

            for (int i = 0; i < count; i++)
            {
                if (results[i] == collider)
                    return true;
            }

            return false;
        }

        public static bool OverlapCapsule(this Collider collider, Vector3 point1, Vector3 point2, float radius)
        {
            if (!collider)
                return false;

            var go = collider.gameObject;
            var scene = go.scene.GetPhysicsScene();
            var myLayer = 1 << go.layer;
            int count = scene.OverlapCapsule(point1, point2, radius, results, myLayer, QueryTriggerInteraction.Collide);

            for (int i = 0; i < count; i++)
            {
                if (results[i] == collider)
                    return true;
            }

            return false;
        }
    }
}
#endif
