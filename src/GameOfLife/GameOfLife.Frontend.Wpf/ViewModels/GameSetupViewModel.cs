using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
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
        private readonly UdpClient _receiverUdpClient;
        private readonly UdpClient _senderUdpClient;
        private readonly DelegateCommand _startGameCommand;
        private readonly DelegateCommand _startHeartbeatsCommand;
        private bool _nameNotSet;
        private string _status;
        public bool IsHost { get; set; }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }

        public string PlayerName { get; set; }

        public bool NameNotSet
        {
            get => _nameNotSet;
            set
            {
                _nameNotSet = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Player> Players { get; set; }
        public GameConfiguration GameConfiguration { get; set; }
        public ICommand StartGameCommand => _startGameCommand;
        public ICommand AddPlayerCommand => _addPlayerCommand;
        public ICommand StartHeartbeatsCommand => _startHeartbeatsCommand;

        public GameSetupViewModel(IGameManager gameManager, IEventAggregator eventAggregator, PlayerProvider playerProvider)
        {
            NameNotSet = true;
            Players = playerProvider.Players;
            _eventAggregator = eventAggregator;
            _playerProvider = playerProvider;
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));

            _receiverUdpClient = new UdpClient(new IPEndPoint(IPAddress.Parse(GetLocalIpAddress()), 10000));
            _senderUdpClient = new UdpClient();

            GameConfiguration = new GameConfiguration {MapHeight = 100, MapWidth = 100, GenerationsPerRound = 1};

            _startGameCommand = new DelegateCommand(StartGameCommandExecuteMethod, StartGameCommandCanExecuteMethod);
            _addPlayerCommand = new DelegateCommand(AddNewPlayerCommandExecute);
            _startHeartbeatsCommand = new DelegateCommand(() =>
            {
                NameNotSet = false;
                _startHeartbeatsCommand.RaiseCanExecuteChanged();
                Task.Run(ReceiveHeartBeatsAsync);
                Task.Run(SendHeartBeats);
            }, () => NameNotSet);
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
            _playerProvider.CurrentPlayer = _playerProvider.Players.First(x => x.Name == PlayerName);
            _gameManager.GenerateGameMap(GameConfiguration);
            AddPlayer();
            _eventAggregator.GetEvent<GameStartedEvent>().Publish();
        }

        private void AddPlayer()
        {
            _playerProvider.Players.AddRange(Players);
        }

        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        private async Task SendHeartBeats()
        {
            try
            {
                while (!_gameManager.Started)
                {
                    await Task.Delay(1000);
                    if (string.IsNullOrEmpty(PlayerName))
                    {
                        continue;
                    }
                    var heartBeat = new HeartBeat {PlayerName = PlayerName};
                    var xmlSerializer = new XmlSerializer(heartBeat.GetType());

                    using (var textWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(textWriter, heartBeat);
                        var txt = textWriter.ToString();
                        var toBytes = Encoding.UTF8.GetBytes(txt);
                        var localAddress = IPAddress.Parse(GetLocalIpAddress()).GetAddressBytes();

                        for (byte i = 1; i < 255; i++)
                        {
                            var ipAddress = new IPAddress(new[]{ localAddress[0], localAddress[1], localAddress[2], i });
                            await _senderUdpClient.SendAsync(toBytes, toBytes.Length, new IPEndPoint(ipAddress, 10000));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Status = e.ToString();
            }
        }

        private async Task ReceiveHeartBeatsAsync()
        {
            try
            {
                while (!_gameManager.Started)
                {
                    var data = await _receiverUdpClient.ReceiveAsync();
                    var str = Encoding.UTF8.GetString(data.Buffer);
                    var serializer = new XmlSerializer(typeof(HeartBeat));
                    HeartBeat result;
                    using (TextReader reader = new StringReader(str))
                    {
                        result = serializer.Deserialize(reader) as HeartBeat;
                    }
                    if (result == null)
                    {
                        Status = "Fehler";
                        continue;
                    }
                    if (Players.Any(x => x.Name == result.PlayerName) == false)
                    {
                        Players.Add(new Player {Name = result.PlayerName, IpAddress = data.RemoteEndPoint.Address});
                        _startGameCommand.RaiseCanExecuteChanged();
                    }
                }
            }
            catch (Exception e)
            {
                Status = e.ToString();
            }
        }
    }

    public class HeartBeat
    {
        public string PlayerName { get; set; }
    }
}