using System;
using System.Text;
using PurrNet.Logging;
using PurrNet.Modules;
using PurrNet.Packing;
using UnityEngine;

namespace PurrNet
{
    [Serializable]
    public class Reference<T> : NetworkModule where T : Component
    {
        [SerializeField] T _reference;

        public T value => _reference;

        public static implicit operator T(Reference<T> reference)
        {
            return reference._reference;
        }

        public void SetReference(T reference)
        {
            var trs = reference ? reference.transform : null;
            var componentIdx = reference ? reference.GetComponentIndex() : -1;
            SetReferenceRpc(trs, componentIdx);
        }

        public override void OnObserverAdded(PlayerID player)
        {
            var trs = _reference ? _reference.transform : null;
            var componentIdx = _reference ? _reference.GetComponentIndex() : -1;

            SetReference(player, trs, componentIdx);
        }

        [TargetRpc]
        private void SetReference(PlayerID player, Transform container, PackedInt componentIdx)
        {
            ReceivedLink(container, componentIdx);
        }

        [ObserversRpc]
        private void SetReferenceRpc(Transform container, PackedInt componentIdx)
        {
            ReceivedLink(container, componentIdx);
        }

        private void ReceivedLink(Transform container, PackedInt componentIdx)
        {
            if (!container || componentIdx == -1)
            {
                _reference = null;
                return;
            }

            try
            {
                _reference = container.gameObject.GetComponentAtIndex<T>(componentIdx);
            }
            catch
            {
                _reference = null;
                return;
            }
        }
    }
}
