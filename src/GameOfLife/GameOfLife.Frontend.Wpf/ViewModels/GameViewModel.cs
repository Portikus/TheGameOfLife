using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GameOfLife.Api;
using GameOfLife.Api.Model;
using GameOfLife.Frontend.Wpf.Events;
using GameOfLife.Frontend.Wpf.Model;
using Prism.Commands;
using Prism.Events;

namespace GameOfLife.Frontend.Wpf.ViewModels
{
    public class GameViewModel
    {
        private readonly DelegateCommand _endTurnCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly IGameManager _gameManager;
        private readonly PlayerProvider _playerProvider;
        private bool gameStarted;
        public Player CurrentPlayer { get; set; }

        public ICommand EndTurnCommand => _endTurnCommand;

        public GameViewModel(IGameManager gameManager, PlayerProvider playerProvider, IEventAggregator eventAggregator)
        {
            _gameManager = gameManager;
            _playerProvider = playerProvider;
            _eventAggregator = eventAggregator;
            _endTurnCommand = new DelegateCommand(EndTurnExecuteMethod);

            _eventAggregator.GetEvent<GameStartedEvent>().Subscribe(OnGameStarted);
        }

        private void OnGameStarted()
        {
            CurrentPlayer = _playerProvider.Players.First();
            _playerProvider.CurrentPlayer = CurrentPlayer;
        }

        private void EndTurnExecuteMethod()
        {
            if (gameStarted)
            {
                GeneratePlayerActions();
            }
            GenerateInitialPlayerSetup();
        }

        private void GenerateInitialPlayerSetup()
        {
            var gameMap = _gameManager.GameMap;
            var playerInitialCoordinates = new List<Coordinate>();
            for (var i = 0; i < gameMap.Tiles.Length; i++)
            {
                for (var j = 0; j < gameMap.Tiles[i].Length; j++)
                {
                    var tile = gameMap.Tiles[i][j];
                    if (tile.Entity?.Owner == CurrentPlayer)
                    {
                        playerInitialCoordinates.Add(new Coordinate {X = i, Y = j});
                    }
                }
            }
            _gameManager.AddPlayer(new PlayerConfiguration {Coordinates = playerInitialCoordinates, Player = CurrentPlayer, StartAttributes = null});
        }

        private void GeneratePlayerActions()
        {
            throw new NotImplementedException();
        }
    }
}