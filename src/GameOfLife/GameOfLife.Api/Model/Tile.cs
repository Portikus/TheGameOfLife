using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class Tile
    {
        public IDictionary<TileAttribute, int> TileAttributes { get; set; }
        public Entity Entity { get; set; }
    }
}