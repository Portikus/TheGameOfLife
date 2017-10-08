using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IGameManager GameManager { get; }
        private readonly List<PlayerAction> _playerActions = new List<PlayerAction>();
        public PlayerProvider PlayerProvider { get; }

        public ICommand EndTurnCommand => _endTurnCommand;

        public GameMapViewModel GameMapViewModel { get; }

        public GameViewModel(IGameManager gameManager, PlayerProvider playerProvider, IEventAggregator eventAggregator, GameMapViewModel gameMapViewModel)
        {
            GameManager = gameManager;
            PlayerProvider = playerProvider;
            GameMapViewModel = gameMapViewModel;
            _endTurnCommand = new DelegateCommand(async ()=> await EndTurnExecuteMethod());

            eventAggregator.GetEvent<GameStartedEvent>().Subscribe(OnGameStarted);
        }

        private void OnGameStarted()
        {
            PlayerProvider.CurrentPlayer = PlayerProvider.Players.First();
        }

        private async Task EndTurnExecuteMethod()
        {
            if (GameManager.Started == false)
            {
                GenerateInitialPlayerSetup();
                RemoveInitialSetup();
                if (PlayerProvider.CurrentPlayer == PlayerProvider.Players.Last())
                {
                    GameManager.Start();
                }
            }
            GeneratePlayerActions();
            await SelectNextPlayer();
        }

        private void RemoveInitialSetup()
        {
            var gameMap = GameManager.GameMap;
            for (var i = 0; i < gameMap.Tiles.Length; i++)
            {
                for (var j = 0; j < gameMap.Tiles[i].Length; j++)
                {
                    gameMap.Tiles[i][j].Entity = null;
                }
            }
        }

        private async Task SelectNextPlayer()
        {
            var currentPlayerIndex = PlayerProvider.Players.IndexOf(PlayerProvider.CurrentPlayer);
            if (currentPlayerIndex == PlayerProvider.Players.Count - 1)
            {
                PlayerProvider.CurrentPlayer = PlayerProvider.Players.First();
                await OneRound();
            }
            else
            {
                PlayerProvider.CurrentPlayer = PlayerProvider.Players[currentPlayerIndex + 1];
            }
        }

        private async Task OneRound()
        {
            for (int i = 0; i < 10; i++)
            {
                GameManager.SimulateGeneration(_playerActions);
                await Task.Delay(200);
            }
            _playerActions.Clear();
        }

        private void GenerateInitialPlayerSetup()
        {
            var gameMap = GameManager.GameMap;
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
            GameManager.AddPlayer(new PlayerConfiguration
            {
                Coordinates = playerInitialCoordinates,
                Player = PlayerProvider.CurrentPlayer,
                StartAttributes = new Dictionary<EntityAttribute, int>
                {
                    [EntityAttribute.MaxNeighboursForDead] = 3,
                    [EntityAttribute.MaxNeighboursForLife] = 3,
                    [EntityAttribute.MinNeighboursForDead] = 3,
                    [EntityAttribute.MinNeighboursForLife] = 2
                }
            });
        }

        private void GeneratePlayerActions()
        {
            _playerActions.Add(PlayerProvider.PlayerAction);
        }
    }
}