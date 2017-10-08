using System;
using System.Collections;
using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class PlayerConfiguration
    {
        public Dictionary<EntityAttribute, int> StartAttributes{ get; set; }
        public List<Coordinate> Coordinates { get; set; }
        public Player Player { get; set; }
    }
}