using System.Collections.Generic;
using GameOfLife.Api.Model;
using Prism.Mvvm;

namespace GameOfLife.Frontend.Wpf.Model
{
    public class PlayerProvider : BindableBase
    {
        private Player _currentPlayer;
        public List<Player> Players { get; }

        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                _currentPlayer = value;
                RaisePropertyChanged();
            }
        }

        public PlayerProvider()
        {
            Players = new List<Player>();
        }
    }
}