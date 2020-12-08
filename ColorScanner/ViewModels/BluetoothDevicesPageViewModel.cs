using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ColorScanner.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace ColorScanner.ViewModels
{
    public class BluetoothDevicesPageViewModel : BaseViewModel
    {
        private IBluetoothService _BluetoothService;
        public IBluetoothService BluetoothService => _BluetoothService ??= App.Resolve<IBluetoothService>();

        public BluetoothDevicesPageViewModel()
        {
        }

        #region -- Public Properties --

        private ObservableCollection<string> _Devices;
        public ObservableCollection<string> Devices
        {
            get => _Devices;
            set => SetProperty(ref _Devices, value);
        }

        private ICommand _SelectDeviceCommand;
        public ICommand SelectDeviceCommand => _SelectDeviceCommand ??= new Command<string>(OnSelectDeviceCommand);

        private ICommand _RefreshCommand;
        public ICommand RefreshCommand => _RefreshCommand ??= new Command(OnRefreshCommand);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            OnRefreshCommand();
        }

        #endregion

        #region -- Private Helpers --

        private async void OnSelectDeviceCommand(string device)
        {
            await NavigationService.GoBackAsync(new NavigationParameters
            {
                { Constants.SelectedDeviceKey, device }
            });
        }

        private async void OnRefreshCommand()
        {
            var devices = await _BluetoothService.GetPairedDevices();
            Devices = new ObservableCollection<string>(devices);
        }

        #endregion
    }
}
