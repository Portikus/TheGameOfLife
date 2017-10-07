using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GameOfLife.Api;
using GameOfLife.Api.Model;
using GameOfLife.Frontend.Wpf.Events;
using GameOfLife.Frontend.Wpf.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace GameOfLife.Frontend.Wpf.ViewModels
{
    public class GameSetupViewModel : BindableBase
    {
        private readonly DelegateCommand _addPlayerCommand;
        private readonly IEventAggregator _eventAggregator;
        private readonly IGameManager _gameManager;
        private readonly PlayerProvider _playerProvider;
        private readonly DelegateCommand _startGameCommand;

        public ObservableCollection<Player> Players { get; set; }
        public GameConfiguration GameConfiguration { get; set; }
        public ICommand StartGameCommand => _startGameCommand;
        public ICommand AddPlayerCommand => _addPlayerCommand;

        public GameSetupViewModel(IGameManager gameManager, IEventAggregator eventAggregator, PlayerProvider playerProvider)
        {
            _eventAggregator = eventAggregator;
            _playerProvider = playerProvider;
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));

            Players = new ObservableCollection<Player>
            {
                new Player {Name = "Jonas"},
                new Player {Name = "Florian"},
                new Player {Name = "Jannik"},
                new Player {Name = "Sebastian"}
            };
            GameConfiguration = new GameConfiguration {MapHeight = 20, MapWidth = 20, GenerationsPerRound = 1};

            _startGameCommand = new DelegateCommand(StartGameCommandExecuteMethod, StartGameCommandCanExecuteMethod);
            _addPlayerCommand = new DelegateCommand(AddNewPlayerCommandExecute);
        }

        private void AddNewPlayerCommandExecute()
        {
            Players.Add(new Player {Name = $"Player{Players.Count}"});
            _startGameCommand.RaiseCanExecuteChanged();
        }

        private bool StartGameCommandCanExecuteMethod()
        {
            return Players.Count >= 2;
        }

        private void StartGameCommandExecuteMethod()
        {
            _gameManager.GenerateGameMap(GameConfiguration);
            AddPlayer();
            _gameManager.Start();
            _eventAggregator.GetEvent<GameStartedEvent>().Publish();
        }

        private void AddPlayer()
        {
            _playerProvider.Players.AddRange(Players);
        }
    }
}