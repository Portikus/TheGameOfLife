namespace GameOfLife.Api.Model
{
    public class Temperature : TileAttribute
    {
        public static int MinTemperature = -10;
        public static int MaxTemperature = 40;
        public Temperature() : base(1, -MinTemperature, MaxTemperature)
        {
        }
    }
}