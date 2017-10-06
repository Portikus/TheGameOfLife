using System.Collections.Generic;

namespace GameOfLife.Api.Model
{
    public class PlayerAction
    {
        public Player Player { get; set; }
        public IDictionary<EntityAttribute,int> AttributeChanges { get; set; }
    }
}