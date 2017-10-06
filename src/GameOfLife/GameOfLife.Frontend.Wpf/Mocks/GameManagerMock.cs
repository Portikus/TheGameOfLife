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
        public event EventHandler<GameFinishedEventArgs> GameFinished;

        public GameMap GenerateGameMap(GameConfiguration gameConfiguration)
        {
            GameMap = new GameMap();
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