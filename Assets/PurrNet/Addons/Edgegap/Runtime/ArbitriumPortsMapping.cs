using System.Collections.Generic;
using Newtonsoft.Json;

namespace PurrNet.Edgegap.Runtime
{
    public class ArbitriumPortsMapping
    {
        [JsonProperty("ports")]
        public Dictionary<string, PortMappingData> ports { get; private set; }
    }
}