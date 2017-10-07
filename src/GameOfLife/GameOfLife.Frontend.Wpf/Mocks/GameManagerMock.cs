using System;
using System.Collections.Generic;
using GameOfLife.Api;
using GameOfLife.Api.Model;

namespace GameOfLife.Frontend.Wpf.Mocks
{
    public class GameManagerMock : IGameManager
    {
        public GameManagerMock()
        {
            Round = 0;
        }

        public GameMap GameMap { get; private set; }
        public int Round { get; }
        public int Generations { get; }
        public bool Started { get; }
        public event EventHandler<GameFinishedEventArgs> GameFinished;
        public event EventHandler<GenerationDoneEventArgs> GenerationDone;
        public event EventHandler<RoundDoneEventArgs> RoundDone;

        public GameMap GenerateGameMap(GameConfiguration gameConfiguration)
        {
            var random = new Random();
            GameMap = new GameMap {Tiles = new Tile[gameConfiguration.MapHeight][]};
            for (var i = 0; i < GameMap.Tiles.Length; i++)
            {
                GameMap.Tiles[i] = new Tile[gameConfiguration.MapWidth];
                for (var j = 0; j < GameMap.Tiles[i].Length; j++)
                {
                    var tile = new Tile();
                    //tile.Temperature.Value = random.Next((int) tile.Temperature.Minimum, (int) tile.Temperature.Maximum);
                    GameMap.Tiles[i][j] = tile;
                }
            }
            var player = new Player {Name = "Florian"};
            var entity1 = new Entity
            {
                Owner = player
            };
            var entity2 = new Entity
            {
                Owner = player
            };
            var entity3 = new Entity
            {
                Owner = player
            };
            var entity4 = new Entity
            {
                Owner = player
            };
            var entity5 = new Entity
            {
                Owner = player
            };
            var entity6 = new Entity
            {
                Owner = player
            };
            entity1.NeighborInfo.HasEastNeighbor = true;
            entity1.NeighborInfo.HasSouthNeighbor = true;

            entity2.NeighborInfo.HasWestNeighbor = true;
            entity2.NeighborInfo.HasSouthNeighbor = true;

            entity3.NeighborInfo.HasEastNeighbor = true;

            entity4.NeighborInfo.HasNorthNeighbor = true;
            entity4.NeighborInfo.HasSouthNeighbor = true;
            entity4.NeighborInfo.HasWestNeighbor = true;
            entity4.NeighborInfo.HasEastNeighbor = true;

            entity5.NeighborInfo.HasNorthNeighbor = true;
            entity5.NeighborInfo.HasWestNeighbor = true;

            entity6.NeighborInfo.HasNorthNeighbor = true;

            GameMap.Tiles[1][1].Entity = entity1;
            GameMap.Tiles[2][1].Entity = entity2;
            GameMap.Tiles[0][2].Entity = entity3;
            GameMap.Tiles[1][2].Entity = entity4;
            GameMap.Tiles[2][2].Entity = entity5;
            GameMap.Tiles[1][3].Entity = entity6;

            return new GameMap();
        }

        public void Start()
        {
        }

        public void AddPlayer(PlayerConfiguration player)
        {
        }

        public void SimulateRound(IEnumerable<PlayerAction> playerActions)
        {
        }
    }
}