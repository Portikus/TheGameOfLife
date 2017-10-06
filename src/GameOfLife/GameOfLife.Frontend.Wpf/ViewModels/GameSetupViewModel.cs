using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GameOfLife.Api;
using GameOfLife.Api.Model;
using Prism.Commands;
using Prism.Mvvm;

namespace GameOfLife.Frontend.Wpf.ViewModels
{
    public class GameSetupViewModel : BindableBase
    {
        private readonly IGameManager _gameManager;

        public GameSetupViewModel(IGameManager gameManager)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));

            Players = new ObservableCollection<Player>();
            GameConfiguration = new GameConfiguration();

            StartGameCommand = new DelegateCommand(StartGameCommandExecuteMethod, StartGameCommandCanExecuteMethod);
        }

        public ObservableCollection<Player> Players { get; set; }
        public GameConfiguration GameConfiguration { get; set; }
        public ICommand StartGameCommand { get; set; }

        private bool StartGameCommandCanExecuteMethod()
        {
            return Players.Count >= 2;
        }

        private void StartGameCommandExecuteMethod()
        {
            _gameManager.GenerateGameMap(GameConfiguration);
            _gameManager.Start();
        }
    }
}