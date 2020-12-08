using System;
using System.Windows.Input;
using ColorScanner.Services;
using ColorScanner.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace ColorScanner.ViewModels
{
    public class ColorsPageViewModel : BaseViewModel
    {
        private IBluetoothService _BluetoothService;
        private IBluetoothService BluetoothService => _BluetoothService ??= App.Resolve<IBluetoothService>();

        public ColorsPageViewModel()
        {
            
        }

        #region -- Public Properties --

        private Color _CurrentColor;
        public Color CurrentColor
        {
            get => _CurrentColor;
            set => SetProperty(ref _CurrentColor, value);
        }

        private string _HEXValue;
        public string HexValue
        {
            get => _HEXValue;
            set => SetProperty(ref _HEXValue, value);
        }

        private string _ConnectedDevice;
        public string ConnectedDevice
        {
            get => _ConnectedDevice;
            set => SetProperty(ref _ConnectedDevice, value);
        }

        private bool _IsConnected;
        public bool IsConnected
        {
            get => _IsConnected;
            set => SetProperty(ref _IsConnected, value);
        }

        private ICommand _ConnectCommand;
        public ICommand ConnectCommand => _ConnectCommand ??= new Command(OnConnectCommand);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.SelectedDeviceKey, out string device))
            {
                ConnectedDevice = device;
                _BluetoothService.Start(ConnectedDevice, 200);
                IsConnected = true;
            }
        }

        #endregion

        #region -- Private Helpers --

        private async void OnConnectCommand()
        {
            if (IsConnected)
            {
                _BluetoothService.Cancel();
                ConnectedDevice = null;
                IsConnected = false;
            }
            else
            {
                await NavigationService.NavigateAsync(nameof(BluetoothDevicesPage));
            }
        }

        #endregion
    }
}

