using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class Entity
    {
        public IDictionary<EntityAttribute,int> EntityAttributes { get; set; }
        public Player Owner { get; set; }
    }
}