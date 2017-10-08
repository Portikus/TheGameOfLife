namespace GameOfLife.Api.Model
{
    public class GameConfiguration
    {
        public int MapHeight { get; set; }
        public int MapWidth { get; set; }
        public int GenerationsPerRound { get; set; }
        public int Seed { get; set; }
    }
}