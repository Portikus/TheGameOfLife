using System;
using System.Collections.Generic;
using GameOfLife.Api.Model;

namespace GameOfLife.Api
{
    public interface IGameManager
    {
        event EventHandler<GameFinishedEventArgs> GameFinished;
        GameMap GenerateGameMap(GameConfiguration gameConfiguration);
        void Start();
        void AddPlayer(PlayerConfiguration player);
        void SimulateRound(IEnumerable<PlayerAction> playerActions);
    }
}