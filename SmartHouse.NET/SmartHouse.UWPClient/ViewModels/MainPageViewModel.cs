using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.Networking.Sockets;
using Windows.UI.Xaml;

namespace SmartHouse.UWPClient.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public Visibility VPNVisible { get { return Get<Visibility>(); } set { Set(value); } }

        public string PingStatus { get { return Get<string>(); } set { Set(value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await Ping();
        }    
        
        public async Task Ping()
        {
            try
            {
                Status = "Checking server";
                VPNVisible = Visibility.Collapsed;

                using (var tcpClient = new StreamSocket())
                {
                    await tcpClient.ConnectAsync(
                        new Windows.Networking.HostName("10.110.166.90"),
                        "80",
                        SocketProtectionLevel.PlainSocket);

                    var localIp = tcpClient.Information.LocalAddress.DisplayName;
                    var remoteIp = tcpClient.Information.RemoteAddress.DisplayName;
                }
                                
                PingStatus = "Connected to server";
            }
            catch (Exception ex)
            {
                PingStatus = $"Error: {ex.Message}";
                VPNVisible = Visibility.Visible;
            }

            Status = "";
        }    
    }
}

