namespace GameOfLife.Api.Model
{
    public class Tile : ModelBase
    {
        private Entity _entity;

        public Temperature Temperature { get; }

        public bool DebugHighlight { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public Entity Entity
        {
            get => _entity;
            set
            {
                _entity = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAlive => Entity != null;

        public Tile()
        {
            Temperature = new Temperature();
            Temperature.PropertyChanged += (sender, args) => RaisePropertyChanged(nameof(Temperature));
        }

        public override string ToString()
        {
            return Entity == null ? "Empty Tile" : $"Player {Entity.Owner.Name}";
        }
    }
}