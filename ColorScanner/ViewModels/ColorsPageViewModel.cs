using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ColorScanner.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace ColorScanner.ViewModels
{
    public class ColorsPageViewModel : BaseViewModel
    {
        private const int DEFAULT_REFRESHING_FREQUENCY = 400;
        private IBluetoothService _BluetoothService;

        private CancellationTokenSource _refreshingCancellationToken;
        private string _RRegex = "R:[0-9]{0,3}";
        private string _GRegex = "G:[0-9]{0,3}";
        private string _BRegex = "B:[0-9]{0,3}";
        private string _FrRegex = "Fr:[0-9]{0,5}";
        private string _FgRegex = "Fg:[0-9]{0,5}";
        private string _FbRegex = "Fb:[0-9]{0,5}";

        //R:231G:228B:234Fr:41666Fg:37037Fb:47619

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

        private int _FRValue;
        public int FRValue
        {
            get => _FRValue;
            set => SetProperty(ref _FRValue, value);
        }

        private int _FGValue;
        public int FGValue
        {
            get => _FGValue;
            set => SetProperty(ref _FGValue, value);
        }

        private int _FBValue;
        public int FBValue
        {
            get => _FBValue;
            set => SetProperty(ref _FBValue, value);
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
                _BluetoothService.Start(ConnectedDevice, DEFAULT_REFRESHING_FREQUENCY, SetDisplayingData);

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

        private Task SetDisplayingData(string data)
        {
            try
            {
                var rRegex = new Regex(_RRegex);
                var gRegex = new Regex(_GRegex);
                var bRegex = new Regex(_BRegex);

                var frRegex = new Regex(_FrRegex);
                var fgRegex = new Regex(_FgRegex);
                var fbRegex = new Regex(_FbRegex);

                var rStr = rRegex.Match(data).Value;
                var gStr = gRegex.Match(data).Value;
                var bStr = bRegex.Match(data).Value;

                var r = int.Parse(rStr.Replace("R:", string.Empty));
                var g = int.Parse(gStr.Replace("G:", string.Empty));
                var b = int.Parse(bStr.Replace("B:", string.Empty));

                var color = System.Drawing.Color.FromArgb(0, r, g, b);
                var hex = $"{color.R:X2}{color.G:X2}{color.B:X2}";

                var frStr = frRegex.Match(data).Value;
                var fgStr = fgRegex.Match(data).Value;
                var fbStr = fbRegex.Match(data).Value;

                var fr = int.Parse(frStr.Replace("Fr:", string.Empty));
                var fg = int.Parse(fgStr.Replace("Fg:", string.Empty));
                var fb = int.Parse(fbStr.Replace("Fb:", string.Empty));

                CurrentColor = Color.FromHex(hex);
                HexValue = hex;

                RValue = r;
                GValue = g;
                BValue = b;

                FRValue = fr;
                FGValue = fg;
                FBValue = fb;

                NoData = false;
            }
            catch
            {
                Debug.WriteLine("Something wrong.");
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}

