using LiteNetLib;
using System.Net;

namespace PurrNet.Transports
{
  public class PeerInfo
  {
    public IPAddress Address { get; set; }

    public int Port { get; set; }

    public int Id { get; set; }

    static public PeerInfo Generate(NetPeer fromNetPeer)
    {
      return new PeerInfo()
      {
        Address = fromNetPeer.Address,
        Port = fromNetPeer.Port,
        Id = fromNetPeer.Id
      };
    }
  }
}