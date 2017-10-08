using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameOfLife.Api.Model
{
    public class PlayerConfiguration
    {
        [XmlIgnore]
        public Dictionary<EntityAttribute, int> StartAttributes { get; set; }

        public List<Coordinate> Coordinates { get; set; }
        public Player Player { get; set; }

        public PlayerConfiguration()
        {
            StartAttributes = new Dictionary<EntityAttribute, int>
            {
                [EntityAttribute.MaxNeighboursForDead] = 6,
                [EntityAttribute.MaxNeighboursForLife] = 7,
                [EntityAttribute.MinNeighboursForDead] = 3,
                [EntityAttribute.MinNeighboursForLife] = 2
            };
        }
    }
}