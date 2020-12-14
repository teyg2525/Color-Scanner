using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ColorScanner.Services;
using ColorScanner.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace ColorScanner.ViewModels
{
    public class ColorsPageViewModel : BaseViewModel
    {
        private const int DEFAULT_REFRESHING_FREQUENCY = 400;
        private IBluetoothService _BluetoothService;

        private CancellationTokenSource _refreshingCancellationToken;
        private string _RRegex = "R:[0-9]{3}";
        private string _GRegex = "G:[0-9]{3}";
        private string _BRegex = "B:[0-9]{3}";

        public ColorsPageViewModel(IBluetoothService bluetoothService)
        {
            _BluetoothService = bluetoothService;
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

        private int _RValue;
        public int RValue
        {
            get => _RValue;
            set => SetProperty(ref _RValue, value);
        }

        private int _GValue;
        public int GValue
        {
            get => _GValue;
            set => SetProperty(ref _GValue, value);
        }

        private int _BValue;
        public int BValue
        {
            get => _BValue;
            set => SetProperty(ref _BValue, value);
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

        private bool _NoData = true;
        public bool NoData
        {
            get => _NoData;
            set => SetProperty(ref _NoData, value);
        }

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

                _refreshingCancellationToken = new CancellationTokenSource();

                Task.Run(() => InitRefreshingLoopAsync(_refreshingCancellationToken.Token));

                IsConnected = true;
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            _refreshingCancellationToken?.Cancel();
        }

        #endregion

        #region -- Private Helpers --

        private async Task InitRefreshingLoopAsync(CancellationToken cancellationToken, int frequency = DEFAULT_REFRESHING_FREQUENCY)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var data = await _BluetoothService.GetCurrentData();

                if (!string.IsNullOrEmpty(data))
                {
                    await SetDisplayingData(data);
                }
                else
                {
                    await SetDefaultValues();
                }
            }
        }

        private Task SetDisplayingData(string data)
        {
            try
            {
                var rRegex = new Regex(_RRegex);
                var gRegex = new Regex(_GRegex);
                var bRegex = new Regex(_BRegex);

                var rStr = rRegex.Match(data).Value;
                var gStr = gRegex.Match(data).Value;
                var bStr = bRegex.Match(data).Value;

                var r = int.Parse(rStr.Replace("R:", string.Empty));
                var g = int.Parse(gStr.Replace("G:", string.Empty));
                var b = int.Parse(bStr.Replace("B:", string.Empty));

                var color = System.Drawing.Color.FromArgb(r, g, b);
                var hex = $"{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";

                CurrentColor = Color.FromHex(hex);
                HexValue = hex;
                RValue = r;
                GValue = g;
                BValue = b;
                NoData = false;
            }
            catch
            {
                Debug.WriteLine("Something wrong.");
            }

            return Task.CompletedTask;
        }

        private Task SetDefaultValues()
        {
            NoData = true;

            CurrentColor = Color.White;

            HexValue = null;
            RValue = 0;
            GValue = 0;
            BValue = 0;

            return Task.CompletedTask;
        }

        #endregion
    }
}

