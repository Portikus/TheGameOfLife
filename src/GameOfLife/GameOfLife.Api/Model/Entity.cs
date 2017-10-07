using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class Entity
    {

        public Entity()
        {
            EntityAttributes = new Dictionary<EntityAttribute, int>();
            EntityAttributes[EntityAttribute.MaxNeighboursForDead] = 9;
            EntityAttributes[EntityAttribute.MinNeighboursForDead] = 0;
            EntityAttributes[EntityAttribute.MaxNeighboursForLife] = 9;
            EntityAttributes[EntityAttribute.MinNeighboursForLife] = 0;
        }
        public IDictionary<EntityAttribute,int> EntityAttributes { get; set; }
        public Player Owner { get; set; }

    }
}