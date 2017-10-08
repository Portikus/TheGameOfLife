using System.Net;

namespace GameOfLife.Api.Model
{
    public class Player
    {
        public bool IsHost { get; set; }
        public string Name { get; set; }
        public IPAddress IpAddress { get; set; }
    }
}