using System;
using GameOfLife.Api.Model;

namespace GameOfLife.Api
{
    public class GameFinishedEventArgs : EventArgs
    {
        public Player Winner { get; set; }

        public GameFinishedEventArgs(Player winner)
        {
            Winner = winner;
        }
    }
}