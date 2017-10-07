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
        private readonly IGameManager _gameManager;
        public  PlayerProvider PlayerProvider { get; }
        private bool _gameStarted;

        public ICommand EndTurnCommand => _endTurnCommand;

        public GameViewModel(IGameManager gameManager, PlayerProvider playerProvider, IEventAggregator eventAggregator)
        {
            _gameManager = gameManager;
            PlayerProvider = playerProvider;
            _endTurnCommand = new DelegateCommand(EndTurnExecuteMethod);

            eventAggregator.GetEvent<GameStartedEvent>().Subscribe(OnGameStarted);
        }

        private void OnGameStarted()
        {
            PlayerProvider.CurrentPlayer = PlayerProvider.Players.First();
        }

        private void EndTurnExecuteMethod()
        {
            if (_gameStarted == false)
            {
                GenerateInitialPlayerSetup();
                if (PlayerProvider.CurrentPlayer == PlayerProvider.Players.Last())
                {
                    _gameManager.Start();
                    _gameStarted = true;
                }
            }
            GeneratePlayerActions();
            SelectNextPlayer();
        }

        private void SelectNextPlayer()
        {
            var currentPlayerIndex = PlayerProvider.Players.IndexOf(PlayerProvider.CurrentPlayer);
            if (currentPlayerIndex == PlayerProvider.Players.Count - 1)
            {
                PlayerProvider.CurrentPlayer = PlayerProvider.Players.First();
            }
            else
            {
                PlayerProvider.CurrentPlayer = PlayerProvider.Players[currentPlayerIndex + 1];
            }
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
                    if (tile.Entity?.Owner == PlayerProvider.CurrentPlayer)
                    {
                        playerInitialCoordinates.Add(new Coordinate {X = i, Y = j});
                    }
                }
            }
            _gameManager.AddPlayer(new PlayerConfiguration {Coordinates = playerInitialCoordinates, Player = PlayerProvider.CurrentPlayer, StartAttributes = null});
        }

        private void GeneratePlayerActions()
        {
        }
    }
}