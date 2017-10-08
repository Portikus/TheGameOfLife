using System.Collections.ObjectModel;
using System.Windows.Input;
using GameOfLife.Api;
using GameOfLife.Api.Model;
using GameOfLife.Frontend.Wpf.Events;
using GameOfLife.Frontend.Wpf.Model;
using Prism.Commands;
using Prism.Events;

namespace GameOfLife.Frontend.Wpf.ViewModels
{
    public class GameMapViewModel
    {
        private readonly IGameManager _gameManager;
        private readonly PlayerProvider _playerProvider;
        private readonly DelegateCommand<Tile> _tileSelectedCommand;

        public ICommand TileSelectedCommand => _tileSelectedCommand;

        public ObservableCollection<ObservableCollection<Tile>> GameMap { get; set; }

        public GameMapViewModel(IGameManager gameManager, IEventAggregator eventAggregator, PlayerProvider playerProvider)
        {
            _gameManager = gameManager;
            _playerProvider = playerProvider;

            GameMap = new ObservableCollection<ObservableCollection<Tile>>();
            _tileSelectedCommand = new DelegateCommand<Tile>(TileSelectedCommandExecuted);
            eventAggregator.GetEvent<GameStartedEvent>().Subscribe(OnGameStarted);
        }

        private void TileSelectedCommandExecuted(Tile tile)
        {
            if (_gameManager.Started)
            {
                var temperatureManipulation = new TemperatureManipulation {Tile = tile, Change = 1};
                _playerProvider.PlayerAction.TemperatureManipulations.Add(temperatureManipulation);
            }
            else
            {
                tile.Entity = tile.IsAlive ? null : new Entity { Owner = _playerProvider.CurrentPlayer };
            }

        }

        private void OnGameStarted()
        {
            ConvertGameMapToViewProjection();
            _tileSelectedCommand.RaiseCanExecuteChanged();
        }

        private void ConvertGameMapToViewProjection()
        {
            foreach (var gameMapRow in _gameManager.GameMap.Tiles)
            {
                var row = new ObservableCollection<Tile>();
                foreach (var tile in gameMapRow)
                {
                    row.Add(tile);
                }
                GameMap.Add(row);
            }
        }
    }
}