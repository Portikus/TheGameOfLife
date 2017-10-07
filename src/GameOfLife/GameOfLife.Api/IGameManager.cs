using System;
using System.Collections.Generic;
using GameOfLife.Api.Model;

namespace GameOfLife.Api
{
    public interface IGameManager
    {
        GameMap GameMap { get; }
        int Generations { get; }
        bool Started { get; }
        event EventHandler<GameFinishedEventArgs> GameFinished;
        event EventHandler<GenerationDoneEventArgs> GenerationDone;
        GameMap GenerateGameMap(GameConfiguration gameConfiguration);
        void Start();
        void AddPlayer(PlayerConfiguration configuration);
        void SimulateGeneration(IEnumerable<PlayerAction> playerActions);
    }
}