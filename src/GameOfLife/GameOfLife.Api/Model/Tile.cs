using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class Tile :ModelBase
    {
        private Entity _entity;
        public IDictionary<TileAttribute, int> TileAttributes { get; set; }

        public Entity Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAlive => Entity != null;

        public override string ToString()
        {
            return Entity == null ? "Empty Tile" : $"Player {Entity.Owner.Name}";
        }
    }
}