using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class Tile
    {
        public IDictionary<TileAttribute, int> TileAttributes { get; set; }
        public Entity Entity { get; set; }

        public bool IsAlive => Entity != null;

        public override string ToString()
        {
            return Entity == null ? "Empty Tile" : $"Player {Entity.Owner.Name}";
        }
    }
}