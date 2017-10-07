namespace GameOfLife.Api.Model
{
    public class Temperature : TileAttribute
    {
        public Temperature() : base(1, -273, 10000)
        {
        }
    }
}