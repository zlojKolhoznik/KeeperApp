using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeeperApp.Authentication;
using KeeperApp.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly SignInManager signInManager;
        private readonly KeeperDbContext dbContext;
        private readonly ResourceLoader resourceLoader;
        private bool isWindowsHelloAvailable;
        private string currentPassword;
        private string newPassword;
        private string confirmPassword;
        private string deleteAccountPassword;
        private bool isWindowsHelloConnected;
        private string changePasswordError;

        public SettingsViewModel(SignInManager signInManager, KeeperDbContext dbContext)
        {
            this.signInManager = signInManager;
            this.dbContext = dbContext;
            resourceLoader = new ResourceLoader();
            CheckWindowsHello();
        }

        public string Title => resourceLoader.GetString("Settings");
        public string WindwsHelloDescription => resourceLoader.GetString("WindwsHelloDescription");
        public string EnableWindowsHelloLabel => resourceLoader.GetString("EnableWindowsHello");
        public string DisableWindowsHelloLabel => resourceLoader.GetString("DisableWindowsHello");
        public string WindowsHelloUnaccessible => resourceLoader.GetString("WindowsHelloUnaccessible");
        public string ChangeMasterPassword => resourceLoader.GetString("ChangeMasterPassword");
        public string CurrentPasswordLabel => resourceLoader.GetString("CurrentPassword");
        public string NewPasswordLabel => resourceLoader.GetString("NewPassword");
        public string ConfirmNewPasswordLabel => resourceLoader.GetString("ConfirmNewPassword");
        public string DeleteAccountLabel => resourceLoader.GetString("DeleteAccount");
        public string DeleteAccountWarning => resourceLoader.GetString("DeleteAccountWarning");
        public string DeleteAccountPrompt => resourceLoader.GetString("DeleteAccountPrompt");
        public string Language => resourceLoader.GetString("Language");
        public string Delete => resourceLoader.GetString("Delete");
        public string Cancel => resourceLoader.GetString("Cancel");

        public bool IsWindowsHelloConnected
        {
            get => isWindowsHelloConnected;
            set
            {
                isWindowsHelloConnected = value;
                OnPropertyChanged();
            }
        }

        public bool IsWindowsHelloAvailable 
        {
            get => isWindowsHelloAvailable;
            set
            {
                isWindowsHelloAvailable = value;
                OnPropertyChanged();
            }
        }

        public string DeleteAccountPassword
        {
            get => deleteAccountPassword;
            set
            {
                deleteAccountPassword = value;
                OnPropertyChanged();
            }
        }

        public string CurrentPassword 
        {
            get => currentPassword; 
            set
            {
                currentPassword = value;
                OnPropertyChanged();
            } 
        }

        public string NewPassword 
        { 
            get => newPassword; 
            set
            {
                newPassword = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmPassword 
        {
            get => confirmPassword;
            set
            {
                confirmPassword = value;
                OnPropertyChanged();
            }
        }

        public string ChangePasswordError
        {
            get => changePasswordError;
            set
            {
                changePasswordError = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ChangePasswordCommand => new(ChangePassword);
        public AsyncRelayCommand DeleteAccountCommand => new(DeleteAccount);
        public AsyncRelayCommand EnableWindowsHelloCommand => new(EnableWindowsHello);
        public AsyncRelayCommand DisableWindowsHelloCommand => new(DisableWindowsHello);

        private async Task EnableWindowsHello()
        {
            IsWindowsHelloConnected = await signInManager.ConnectWindowsHelloAsync();
        }

        private async Task DisableWindowsHello()
        {
            IsWindowsHelloConnected = !await signInManager.DisconnectWindowsHello();
        }

        private void ChangePassword()
        {
            if (NewPassword == ConfirmPassword)
            {
                bool changed = signInManager.ChangePassword(signInManager.CurrentUserName, CurrentPassword, NewPassword);
                if (changed)
                {
                    CurrentPassword = "";
                    NewPassword = "";
                    ConfirmPassword = "";
                    ChangePasswordError = resourceLoader.GetString("PasswordChangedSuccessfully");
                }
                else
                {
                    ChangePasswordError = resourceLoader.GetString("IncorrectPassword");
                }
            }
            else
            {
                ChangePasswordError = resourceLoader.GetString("PasswordMismatch");
            }
        }

        private async Task DeleteAccount()
        {
            if (signInManager.Unregister(signInManager.CurrentUserName, DeleteAccountPassword))
            {
                if (await signInManager.IsWindowsHelloConnectedAsync())
                {
                    await signInManager.DisconnectWindowsHello(true);
                }
                var recordsToDelete = dbContext.GetRecordsForUser(signInManager.CurrentUserName);
                foreach (var record in recordsToDelete)
                {
                    dbContext.Remove(record);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        private async void CheckWindowsHello()
        {
            IsWindowsHelloAvailable = await WindowsHelloHelper.IsWindowsHelloAvailableAsync();
            IsWindowsHelloConnected = await signInManager.IsWindowsHelloConnectedAsync();
        }
    }
}
