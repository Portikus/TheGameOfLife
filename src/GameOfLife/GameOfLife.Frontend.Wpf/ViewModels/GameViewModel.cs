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
        private readonly List<PlayerAction> playerActions = new List<PlayerAction>();
        private bool _gameStarted;
        public PlayerProvider PlayerProvider { get; }

        public ICommand EndTurnCommand => _endTurnCommand;

        public GameMapViewModel GameMapViewModel { get; }

        public GameViewModel(IGameManager gameManager, PlayerProvider playerProvider, IEventAggregator eventAggregator, GameMapViewModel gameMapViewModel)
        {
            _gameManager = gameManager;
            PlayerProvider = playerProvider;
            GameMapViewModel = gameMapViewModel;
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
                RemoveInitialSetup();
                if (PlayerProvider.CurrentPlayer == PlayerProvider.Players.Last())
                {
                    _gameManager.Start();
                    _gameStarted = true;
                }
            }
            GeneratePlayerActions();
            SelectNextPlayer();
        }

        private void RemoveInitialSetup()
        {
            var gameMap = _gameManager.GameMap;
            for (var i = 0; i < gameMap.Tiles.Length; i++)
            {
                for (var j = 0; j < gameMap.Tiles[i].Length; j++)
                {
                    gameMap.Tiles[i][j].Entity = null;
                }
            }
        }

        private void SelectNextPlayer()
        {
            var currentPlayerIndex = PlayerProvider.Players.IndexOf(PlayerProvider.CurrentPlayer);
            if (currentPlayerIndex == PlayerProvider.Players.Count - 1)
            {
                PlayerProvider.CurrentPlayer = PlayerProvider.Players.First();
                _gameManager.SimulateRound(playerActions);
                playerActions.Clear();
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
            _gameManager.AddPlayer(new PlayerConfiguration
            {
                Coordinates = playerInitialCoordinates,
                Player = PlayerProvider.CurrentPlayer,
                StartAttributes = new Dictionary<EntityAttribute, int>
                {
                    [EntityAttribute.MaxNeighboursForDead] = 6,
                    [EntityAttribute.MaxNeighboursForLife] = 7,
                    [EntityAttribute.MinNeighboursForDead] = 1,
                    [EntityAttribute.MinNeighboursForLife] = 0
                }
            });
        }

        private void GeneratePlayerActions()
        {
            playerActions.Add(new PlayerAction {Player = PlayerProvider.CurrentPlayer});
        }
    }
}