using System.Collections.ObjectModel;
using GameOfLife.Api;
using GameOfLife.Api.Model;
using GameOfLife.Frontend.Wpf.Events;
using Prism.Events;

namespace GameOfLife.Frontend.Wpf.ViewModels
{
    public class GameMapViewModel
    {
        private readonly IGameManager _gameManager;
        public ObservableCollection<ObservableCollection<Tile>> GameMap { get; set; }

        public GameMapViewModel(IGameManager gameManager, IEventAggregator eventAggregator)
        {
            _gameManager = gameManager;

            GameMap = new ObservableCollection<ObservableCollection<Tile>>();
            eventAggregator.GetEvent<GameStartedEvent>().Subscribe(OnGameStarted);
        }

        private void OnGameStarted()
        {
            ConvertGameMapToViewProjection();
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