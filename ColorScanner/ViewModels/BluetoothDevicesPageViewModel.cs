using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ColorScanner.Services;
using ColorScanner.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace ColorScanner.ViewModels
{
    public class BluetoothDevicesPageViewModel : BaseViewModel
    {
        private IBluetoothService _BluetoothService;

        public BluetoothDevicesPageViewModel(IBluetoothService bluetoothService)
        {
            _BluetoothService = bluetoothService;
        }

        #region -- Public Properties --

        private ObservableCollection<string> _Devices;
        public ObservableCollection<string> Devices
        {
            get => _Devices;
            set => SetProperty(ref _Devices, value);
        }

        private bool _IsRefreshing;
        public bool IsRefreshing
        {
            get => _IsRefreshing;
            set => SetProperty(ref _IsRefreshing, value);
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
            var parameters = new NavigationParameters
            {
                { Constants.SelectedDeviceKey, device }
            };

            await NavigationService.NavigateAsync(nameof(ColorsPage), parameters, useModalNavigation: false, animated: true);
        }

        private async void OnRefreshCommand()
        {
            IsRefreshing = true;
            var devices = await _BluetoothService.GetPairedDevices();
            Devices = new ObservableCollection<string>(devices);
            IsRefreshing = false;
        }

        #endregion
    }
}
