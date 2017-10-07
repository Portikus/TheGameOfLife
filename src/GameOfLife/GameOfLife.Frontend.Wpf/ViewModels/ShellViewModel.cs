using GameOfLife.Frontend.Wpf.Events;
using Prism.Events;
using Prism.Mvvm;

namespace GameOfLife.Frontend.Wpf.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        public GameSetupViewModel GameSetupViewModel { get; }
        public GameMapViewModel GameMapViewModel { get; }
        private bool _gameStarted;

        public bool GameStarted
        {
            get => _gameStarted;
            set
            {
                _gameStarted = value;
                RaisePropertyChanged();
            }
        }
        

        public ShellViewModel(IEventAggregator eventAggregator, GameSetupViewModel gameSetupViewModel, GameMapViewModel gameMapViewModel)
        {
            GameSetupViewModel = gameSetupViewModel;
            GameMapViewModel = gameMapViewModel;
            eventAggregator.GetEvent<GameStartedEvent>().Subscribe(OnGameStarted);
        }

        private void OnGameStarted()
        {
            GameStarted = true;
        }
    }
}