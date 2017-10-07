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
    }
}