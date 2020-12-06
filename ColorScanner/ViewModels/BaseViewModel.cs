using System;
using Prism.Mvvm;

namespace ColorScanner.ViewModels
{
    public class BaseViewModel : BindableBase, IViewActionsHandler
    {
        public BaseViewModel()
        {
        }

        public virtual void OnAppearing()
        {

        }

        public virtual void OnDisappearing()
        {

        }
    }
}
