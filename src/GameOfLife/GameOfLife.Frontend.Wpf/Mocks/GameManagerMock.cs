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
            GameMap = new GameMap {Tiles = new Tile[gameConfiguration.MapHeight][]};
            for (var i = 0; i < GameMap.Tiles.Length; i++)
            {
                GameMap.Tiles[i] = new Tile[gameConfiguration.MapWidth];
                for (var j = 0; j < GameMap.Tiles[i].Length; j++)
                {
                    GameMap.Tiles[i][j] = new Tile();
                }
            }
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