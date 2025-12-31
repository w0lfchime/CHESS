using System;
using UnityEngine;

namespace PurrNet.Transports
{
    [Serializable]
    public class AutomaticCloudSetups
    {
#pragma warning disable 0414

        [SerializeField] private bool _adaptToEdgegap = true;

#pragma warning restore 0414

#if EDGEGAP_PURRNET_SUPPORT && UNITY_SERVER && !UNITY_EDITOR
        public bool adaptToEdgegap => _adaptToEdgegap;
#else
        public bool adaptToEdgegap => false;
#endif
    }
}
