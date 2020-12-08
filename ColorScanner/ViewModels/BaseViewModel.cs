using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Prism.Mvvm;
using Prism.Navigation;

namespace ColorScanner.ViewModels
{
    public class BaseViewModel : BindableBase, IViewActionsHandler, INavigatedAware, IInitializeAsync
    {
        private IUserDialogs _UserDialogs;
        protected IUserDialogs UserDialogs => _UserDialogs ??= App.Resolve<IUserDialogs>();

        private INavigationService _NavigationService;
        protected INavigationService NavigationService => _NavigationService ??= App.Resolve<INavigationService>();

        public BaseViewModel()
        {
        }

        public virtual void OnAppearing()
        {

        }

        public virtual void OnDisappearing()
        {

        }

        public virtual Task InitializeAsync(INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }
    }
}
