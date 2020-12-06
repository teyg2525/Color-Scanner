using System;
namespace ColorScanner.ViewModels
{
    public interface IViewActionsHandler
    {
        void OnAppearing();
        void OnDisappearing();
    }
}
