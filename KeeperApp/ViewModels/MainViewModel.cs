using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeeperApp.Security.Authentication;
using Microsoft.UI.Xaml;
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
            signInManager.EmailConfirmationStatusChanged += (s, e) => OnPropertyChanged(nameof(SettingsInfoBadgeVisibility));
        }

        public string Username => signInManager.CurrentUserName;

        public string Home => resourceLoader.GetString("Home");
        public string Exit => resourceLoader.GetString("Exit");
        public string SignOut => resourceLoader.GetString("SignOut");
        public string PasswordAnalysis => resourceLoader.GetString("PasswordAnalysis");
        public string Settings => resourceLoader.GetString("Settings");
        public Visibility SettingsInfoBadgeVisibility => signInManager.IsEmailConfirmed ? Visibility.Collapsed : Visibility.Visible;

        public RelayCommand ExitCommand => new(App.Current.Exit);

        public RelayCommand SignOutCommand => new(signInManager.SignOut);
    }
}
