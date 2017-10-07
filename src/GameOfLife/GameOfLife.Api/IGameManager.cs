using System;
using System.Collections.Generic;
using GameOfLife.Api.Model;

namespace GameOfLife.Api
{
    public interface IGameManager
    {
        GameMap GameMap { get; }
        int Round { get;}
        int Started { get; }
        event EventHandler<GameFinishedEventArgs> GameFinished;
        event EventHandler<GenerationDoneEventArgs> GenerationDone;
        event EventHandler<RoundDoneEventArgs> RoundDone;
        GameMap GenerateGameMap(GameConfiguration gameConfiguration);
        void Start();
        void AddPlayer(PlayerConfiguration configuration);
        void SimulateRound(IEnumerable<PlayerAction> playerActions);
    }
}