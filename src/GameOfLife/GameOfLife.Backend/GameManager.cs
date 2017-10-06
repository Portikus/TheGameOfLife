using GameOfLife.Api;
using System;
using System.Collections.Generic;
using System.Text;
using GameOfLife.Api.Model;

namespace GameOfLife.Backend
{
    public class GameManager : IGameManager
    {
        private ICollection<Coordinate> _clearTiles;

        public ICollection<Player> PlayerList { get; set; }

        public GameMap GameMap { get; private set; }

        public int Round => throw new NotImplementedException();

        public event EventHandler<GameFinishedEventArgs> GameFinished;

        public GameManager()
        {
            PlayerList = new List<Player>();
            _clearTiles = new List<Coordinate>();
        }

        public void AddPlayer(PlayerConfiguration configuration)
        {
            PlayerList.Add(configuration.Player);
            foreach (var coordinate in configuration.Coordinates)
            {
                if (GameMap.Tiles[coordinate.X][coordinate.Y].Entity != null)
                {
                    _clearTiles.Add(coordinate);
                }
                GameMap.Tiles[coordinate.X][coordinate.Y].Entity = new Entity()
                {
                    Owner = configuration.Player,
                    EntityAttributes = configuration.StartAttributes
                };
            }
        }

        public GameMap GenerateGameMap(GameConfiguration gameConfiguration)
        {
            GameMap = new GameMap();
            GameMap.Tiles = new Tile[gameConfiguration.MapWidth][];
            for (int i = 0; i < GameMap.Tiles.Length; i++)
            {
                GameMap.Tiles[i] = new Tile[gameConfiguration.MapHeight];
                for (int j = 0; j < GameMap.Tiles[i].Length; j++)
                {
                    GameMap.Tiles[i][j] = new Tile();
                }
            }

            return GameMap;
        }

        public void SimulateRound(IEnumerable<PlayerAction> playerActions)
        {
            
        }

        public void Start()
        {
            foreach (var coordinate in _clearTiles)
            {
                GameMap.Tiles[coordinate.X][coordinate.Y].Entity = null;
            }
        }
    }
}
