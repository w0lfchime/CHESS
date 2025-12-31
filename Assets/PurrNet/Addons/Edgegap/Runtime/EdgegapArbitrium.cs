using System;

namespace PurrNet.Edgegap.Runtime
{
    public struct EdgegapArbitrium
    {
        public ArbitriumPortsMapping arbitriumPortsMapping;

        public bool TryGetPort(string protocol, int index, out int port)
        {
            port = 0;

            if (arbitriumPortsMapping == null)
                return false;
            int counter = 0;
            foreach (var (_, data) in arbitriumPortsMapping.ports)
            {
                if (string.Compare(data.protocol, protocol, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    if (counter == index)
                    {
                        port = data.internalPort;
                        return true;
                    }

                    ++counter;
                }
            }

            return false;
        }
    }
}
