using System;
using ColorScanner.ViewModels;
using Prism.Mvvm;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ColorScanner.Views
{
    public class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        #region -- Overrides --

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is IViewActionsHandler vm)
            {
                vm?.OnAppearing();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is IViewActionsHandler vm)
            {
                vm?.OnDisappearing();
            }
        }

        #endregion
    }
}
