using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;
using Prism.Commands;
using Prism.Mvvm;

namespace GameOfLife.Frontend.Wpf.ViewModels
{
    public class LobbyViewModel : BindableBase
    {
        private readonly DelegateCommand _delegateCommand;
        private UdpClient _receiverUdpClient;
        private UdpClient _senderUdpClient;
        private string _status;
        public string LocalAddress { get; set; }
        public ObservableCollection<string> LobbyPlayer { get; set; }

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

        public ICommand StartCommand => _delegateCommand;

        public LobbyViewModel()
        {
            _delegateCommand = new DelegateCommand(StartCommandExecute);
            LobbyPlayer = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(LobbyPlayer, this);
            LocalAddress = "192.168.80.198";
        }

        private void StartCommandExecute()
        {
            _receiverUdpClient = new UdpClient(new IPEndPoint(IPAddress.Parse(LocalAddress), 10000));
            _senderUdpClient = new UdpClient();
            Task.Run(ReceiveHeartBeatsAsync);
            Task.Run(SendHeartBeats);
        }

        private async Task SendHeartBeats()
        {
            try
            {
                while (true)
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
                        for (byte i = 1; i < 255; i++)
                        {
                            await _senderUdpClient.SendAsync(toBytes, toBytes.Length, new IPEndPoint(new IPAddress(new byte[] { 192, 168, 80, i }), 10000));
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
                //udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                //udpClient.ExclusiveAddressUse = false; // only if you want to send/receive on same machine.
                while (true)
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
                    if (LobbyPlayer.Contains(result.PlayerName) == false)
                    {
                        LobbyPlayer.Add(result.PlayerName + data.RemoteEndPoint.Address);
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