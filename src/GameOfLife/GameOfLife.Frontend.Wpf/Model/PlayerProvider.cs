using System.Collections.Generic;
using GameOfLife.Api.Model;
using Prism.Mvvm;

namespace GameOfLife.Frontend.Wpf.Model
{
    public class PlayerProvider : BindableBase
    {
        private Player _currentPlayer;
        public List<Player> Players { get; }
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
            Players = new List<Player>();
            PlayerAction = new PlayerAction();
        }
    }
}