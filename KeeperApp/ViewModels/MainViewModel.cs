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
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly SignInManager signInManager;
        private readonly ResourceLoader resourceLoader;

        public MainViewModel(SignInManager signInManager)
        {
            this.signInManager = signInManager;
            resourceLoader = new ResourceLoader();
        }

        public string Username => signInManager.CurrentUserName;

        public string Home => resourceLoader.GetString("Home");
        public string Exit => resourceLoader.GetString("Exit");
        public string SignOut => resourceLoader.GetString("SignOut");
        public string PasswordAnalysis => resourceLoader.GetString("PasswordAnalysis");

        public RelayCommand ExitCommand => new(App.Current.Exit);

        public RelayCommand SignOutCommand => new(signInManager.SignOut);
    }
}
