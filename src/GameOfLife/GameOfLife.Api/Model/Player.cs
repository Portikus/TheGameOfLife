using System.Net;
using System.Xml.Serialization;

namespace GameOfLife.Api.Model
{
    public class Player
    {
        public bool IsHost { get; set; }
        public string Name { get; set; }
        [XmlIgnore]
        public IPAddress IpAddress { get; set; }
        public PlayerConfiguration PlayerConfiguration { get; set; }
    }
}