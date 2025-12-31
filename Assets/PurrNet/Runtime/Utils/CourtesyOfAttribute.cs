using System;
using UnityEngine;

namespace PurrNet.Contributors
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CourtesyOfAttribute : Attribute
    {
        public string name { get; private set; }
        public string url { get; private set; }

        public CourtesyOfAttribute(string name, string url)
        {
            this.name = name;
            this.url = url;
        }
    }
}
