using System.Collections.Generic;
using GameOfLife.Api.Model;

namespace GameOfLife.Frontend.Wpf.Model
{
    public class PlayerProvider
    {
        public List<Player> Players { get; }
        public Player CurrentPlayer { get; set; }

        public PlayerProvider()
        {
            Players = new List<Player>();
        }
    }
}
