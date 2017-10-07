using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class Entity : ModelBase
    {
        private Player _owner;
        public IDictionary<EntityAttribute, int> EntityAttributes { get; set; }

        public Player Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                RaisePropertyChanged();
            }
        }
    }
}