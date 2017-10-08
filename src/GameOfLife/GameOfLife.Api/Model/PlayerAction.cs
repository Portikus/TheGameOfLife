using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class PlayerAction
    {
        public Player Player { get; set; }
        public IList<TemperatureManipulation> TemperatureManipulations { get; set; }
    }
}