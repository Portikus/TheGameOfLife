using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Prism.Mvvm;

namespace GameOfLife.Frontend.Wpf.ViewModels
{
    public class LobbyViewModel : BindableBase
    {
        private string _status;
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

        public LobbyViewModel()
        {
            LobbyPlayer = new ObservableCollection<string>();
            Task.Run(ReceiveHeartBeatsAsync);
            Task.Run(SendHeartBeats);
        }

        private async Task SendHeartBeats()
        {
            try
            {
                var udpClient = new UdpClient();
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
                        await udpClient.SendAsync(toBytes, toBytes.Length, new IPEndPoint(new IPAddress(new byte[] {192, 168, 80, 255}), 10000));
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
                var udpClient = new UdpClient(10000);
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                //udpClient.ExclusiveAddressUse = false; // only if you want to send/receive on same machine.
                while (true)
                {
                    var data = await udpClient.ReceiveAsync();
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
                    if (LobbyPlayer.Contains(result.PlayerName))
                    {
                        LobbyPlayer.Add(result.PlayerName);
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