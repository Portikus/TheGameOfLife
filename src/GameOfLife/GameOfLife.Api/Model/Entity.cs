using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class Entity
    {
        public Temperature IdealTemperature { get; set; }
        public IDictionary<EntityAttribute, int> EntityAttributes { get; set; }
        public Player Owner { get; set; }
        public NeighborInfo NeighborInfo { get; }

        public Entity()
        {
            NeighborInfo = new NeighborInfo();
            EntityAttributes = new Dictionary<EntityAttribute, int>
            {
                [EntityAttribute.MaxNeighboursForDead] = 9,
                [EntityAttribute.MinNeighboursForDead] = 0,
                [EntityAttribute.MaxNeighboursForLife] = 9,
                [EntityAttribute.MinNeighboursForLife] = 0
            };
        }
    }
}