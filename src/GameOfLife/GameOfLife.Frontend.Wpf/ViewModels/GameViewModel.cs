using System;
using System.Collections.Generic;
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
    public class GameViewModel : BindableBase
    {
        private readonly DelegateCommand _endTurnCommand;
        private readonly List<PlayerAction> _myPlayerActions = new List<PlayerAction>();
        private readonly List<PlayerAction> _otherPlayerActions = new List<PlayerAction>();
        private string _status;
        public IGameManager GameManager { get; }
        public PlayerProvider PlayerProvider { get; }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }

        public ICommand EndTurnCommand => _endTurnCommand;

        public GameMapViewModel GameMapViewModel { get; }

        public GameViewModel(IGameManager gameManager, PlayerProvider playerProvider, IEventAggregator eventAggregator, GameMapViewModel gameMapViewModel)
        {
            GameManager = gameManager;
            PlayerProvider = playerProvider;
            GameMapViewModel = gameMapViewModel;
            _endTurnCommand = new DelegateCommand(EndTurnExecuteMethod);

            eventAggregator.GetEvent<GameStartedEvent>().Subscribe(OnGameStarted);
        }

        private void OnGameStarted()
        {
        }


        private void EndTurnExecuteMethod()
        {
            if (GameManager.Started == false)
            {
                GenerateInitialPlayerSetup();
                RemoveInitialSetup();
                if (PlayerProvider.CurrentPlayer.IsHost)
                {
                    RunHost();
                }
                else
                {
                    RunClient();
                }
            }
            GeneratePlayerActions();
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

        private void OneRound(IEnumerable<PlayerAction> playerActions)
        {
            GameManager.SimulateGeneration(playerActions);
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
            PlayerProvider.PlayerConfigurations.Add(new PlayerConfiguration
            {
                Coordinates = playerInitialCoordinates,
                Player = PlayerProvider.CurrentPlayer
            });
        }

        private void GeneratePlayerActions()
        {
            lock (this)
            {
                _myPlayerActions.Add(PlayerProvider.PlayerAction);
            }
        }

        private void RunClient()
        {
            Task.Run(ReceivePlayerActions);
            Task.Run(SendPlayerActionsToHost);
        }

        private void RunHost()
        {
            Task.Run(ReceivePlayerActions);
            Task.Run(SendPlayerActionsToClients);
        }

        private async Task SendPlayerActionsToHost()
        {
            try
            {
                var senderUdpClient = new UdpClient();
                while (GameManager.Started)
                {
                    await Task.Delay(1000);
                    List<PlayerAction> playerActions;
                    lock (this)
                    {
                        if (_myPlayerActions.Any())
                        {
                            continue;
                        }

                        playerActions = new List<PlayerAction>(_myPlayerActions);
                        _myPlayerActions.Clear();
                    }
                    var playerActionsProvider = new PlayerActionsProvider
                    {
                        PlayerActions = playerActions
                    };
                    var xmlSerializer = new XmlSerializer(playerActionsProvider.GetType());

                    using (var textWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(textWriter, playerActionsProvider);
                        var txt = textWriter.ToString();
                        var toBytes = Encoding.UTF8.GetBytes(txt);
                        await senderUdpClient.SendAsync(toBytes, toBytes.Length, new IPEndPoint(PlayerProvider.Players.First(x => x.IsHost).IpAddress, 10001));
                    }
                }
            }
            catch (Exception e)
            {
                Status = e.ToString();
            }
        }

        private async Task SendPlayerActionsToClients()
        {
            try
            {
                var senderUdpClient = new UdpClient();
                while (!GameManager.Started)
                {
                    await Task.Delay(1000);
                    if (PlayerProvider.Players.Count != PlayerProvider.PlayerConfigurations.Count)
                    {
                        continue;
                    }
                    var configurationsProvider = new PlayerConfigurationsProvider
                    {
                        PlayerConfiguration = PlayerProvider.PlayerConfigurations
                    };
                    var xmlSerializer = new XmlSerializer(configurationsProvider.GetType());

                    using (var textWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(textWriter, configurationsProvider);
                        var txt = textWriter.ToString();
                        var toBytes = Encoding.UTF8.GetBytes(txt);
                        foreach (var ip in PlayerProvider.Players.Where(x => !x.IsHost).Select(x => x.IpAddress))
                        {
                            await senderUdpClient.SendAsync(toBytes, toBytes.Length, new IPEndPoint(ip, 10001));
                        }
                    }
                    GameManager.Start();
                }
                while (GameManager.Started)
                {
                    await Task.Delay(1000);
                    List<PlayerAction> playerActions;
                    lock (this)
                    {
                        if (_myPlayerActions.Any())
                        {
                            continue;
                        }
                        playerActions = new List<PlayerAction>(_otherPlayerActions);
                        _otherPlayerActions.Clear();
                        playerActions.AddRange(_myPlayerActions);
                        _myPlayerActions.Clear();
                    }
                    var playerActionsProvider = new PlayerActionsProvider
                    {
                        PlayerActions = playerActions
                    };
                    var xmlSerializer = new XmlSerializer(playerActionsProvider.GetType());

                    using (var textWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(textWriter, playerActionsProvider);
                        var txt = textWriter.ToString();
                        var toBytes = Encoding.UTF8.GetBytes(txt);
                        foreach (var ip in PlayerProvider.Players.Where(x => !x.IsHost).Select(x => x.IpAddress))
                        {
                            await senderUdpClient.SendAsync(toBytes, toBytes.Length, new IPEndPoint(ip, 10001));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Status = e.ToString();
            }
        }

        private async Task ReceivePlayerActions()
        {
            try
            {
                var receiverUdpClient = new UdpClient(10001);
                while (!GameManager.Started)
                {
                    var data = await receiverUdpClient.ReceiveAsync();
                    var str = Encoding.UTF8.GetString(data.Buffer);
                    var serializer = new XmlSerializer(typeof(PlayerConfiguration));
                    PlayerConfiguration result;
                    using (TextReader reader = new StringReader(str))
                    {
                        result = serializer.Deserialize(reader) as PlayerConfiguration;
                    }
                    if (result == null)
                    {
                        Status = "Fehler";
                        continue;
                    }
                    PlayerProvider.PlayerConfigurations.Add(result);

                    if (PlayerProvider.Players.Count != PlayerProvider.PlayerConfigurations.Count)
                    {
                        if (PlayerProvider.CurrentPlayer.IsHost == false)
                        {
                            GameManager.Start();
                        }
                    }
                }
                while (GameManager.Started)
                {
                    var data = await receiverUdpClient.ReceiveAsync();
                    var str = Encoding.UTF8.GetString(data.Buffer);
                    var serializer = new XmlSerializer(typeof(PlayerActionsProvider));
                    PlayerActionsProvider result;
                    using (TextReader reader = new StringReader(str))
                    {
                        result = serializer.Deserialize(reader) as PlayerActionsProvider;
                    }
                    if (result == null)
                    {
                        Status = "Fehler";
                        continue;
                    }
                    if (PlayerProvider.CurrentPlayer.IsHost)
                    {
                        _otherPlayerActions.AddRange(result.PlayerActions);
                    }
                    OneRound(_otherPlayerActions.Where(x => x.Player.Name != PlayerProvider.CurrentPlayer.Name));
                }
            }
            catch (Exception e)
            {
                Status = e.ToString();
            }
        }

        public class PlayerActionsProvider
        {
            public List<PlayerAction> PlayerActions { get; set; }
        }

        public class PlayerConfigurationsProvider
        {
            public List<PlayerConfiguration> PlayerConfiguration { get; set; }
        }
    }
}