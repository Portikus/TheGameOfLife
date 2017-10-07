namespace GameOfLife.Api.Model
{
    public class NeighborInfo
    {
        public bool HasWestNeighbor { get; set; }
        public bool HasNorthNeighbor { get; set; }
        public bool HasEastNeighbor { get; set; }
        public bool HasSouthNeighbor { get; set; }
    }
}