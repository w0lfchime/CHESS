using UnityEngine;

namespace PurrNet
{
    [System.Serializable]
    public struct OwnershipComponentToggle
    {
        public Component target;
        public bool activeAsOwner;
    }

    [System.Serializable]
    public struct OwnershipGameObjectToggle
    {
        public GameObject target;
        public bool activeAsOwner;
    }
}
