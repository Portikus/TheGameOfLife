using System.Collections.ObjectModel;
using System.Windows.Data;
using GameOfLife.Api.Model;
using Prism.Mvvm;

namespace GameOfLife.Frontend.Wpf.Model
{
    public class PlayerProvider : BindableBase
    {
        private Player _currentPlayer;
        public ObservableCollection<Player> Players { get; }
        public PlayerAction PlayerAction { get; private set; }

        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                _currentPlayer = value;
                UpdatePlayerActions();
                RaisePropertyChanged();
            }
        }

        private void UpdatePlayerActions()
        {
            PlayerAction = new PlayerAction() {Player = CurrentPlayer, TemperatureManipulations = new List<TemperatureManipulation>()};
        }

        public PlayerProvider()
        {
            Players = new ObservableCollection<Player>();
            BindingOperations.EnableCollectionSynchronization(Players, this);
            PlayerAction = new PlayerAction();
        }
    }
}