using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeeperApp.Authentication;
using KeeperApp.Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly SignInManager signInManager;

        public MainViewModel(SignInManager signInManager)
        {
            this.signInManager = signInManager;
        }

        public string Username => signInManager.CurrentUserName;

        public RelayCommand ExitCommand => new(App.Current.Exit);

        public RelayCommand SignOutCommand => new(signInManager.SignOut);
    }
}
