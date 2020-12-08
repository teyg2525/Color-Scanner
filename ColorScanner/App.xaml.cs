﻿using System;
using Acr.UserDialogs;
using ColorScanner.Services;
using ColorScanner.ViewModels;
using ColorScanner.Views;
using Prism.Ioc;
using Prism.Unity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColorScanner
{
    public partial class App : PrismApplication
    {
        public static T Resolve<T>()
        {
            return (Application.Current as App).Container.Resolve<T>();
        }

        public App()
            : this(null)
        {
        }

        public App(Prism.IPlatformInitializer initializer = null)
            : base(initializer)
        {
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            MainPage = new ColorsPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(UserDialogs.Instance);

            containerRegistry.RegisterForNavigation<ColorsPage, ColorsPageViewModel>(nameof(ColorsPage));
            containerRegistry.RegisterForNavigation<BluetoothDevicesPage, BluetoothDevicesPageViewModel>(nameof(BluetoothDevicesPage));
        }
    }
}
