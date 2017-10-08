namespace GameOfLife.Api.Model
{
    public class GameConfiguration : ModelBase
    {
        private int _seed;
        public int MapHeight { get; set; }
        public int MapWidth { get; set; }
        public int GenerationsPerRound { get; set; }

        public int Seed
        {
            get { return _seed; }
            set
            {
                _seed = value;
                RaisePropertyChanged();
            }
        }
    }
}