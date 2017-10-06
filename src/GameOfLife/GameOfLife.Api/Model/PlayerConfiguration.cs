using System;
using System.Collections;
using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class PlayerConfiguration
    {
        public IDictionary<EntityAttribute, int> StartAttributes{ get; set; }
        public IEnumerable<Coordinate> Coordinates { get; set; }
        public Player Player { get; set; }
    }
}