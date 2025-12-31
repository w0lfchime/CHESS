using System;

namespace PurrNet.Contributors
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContributorAttribute : Attribute
    {
        public string name { get; private set; }
        public string url { get; private set; }

        public ContributorAttribute(string name, string url)
        {
            this.name = name;
            this.url = url;
        }
    }
}
