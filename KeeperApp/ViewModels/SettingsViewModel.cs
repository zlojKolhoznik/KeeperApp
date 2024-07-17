using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeeperApp.Authentication;
using KeeperApp.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly SignInManager signInManager;
        private readonly KeeperDbContext dbContext;
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
            CheckWindowsHello();
        }

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
            IsWindowsHelloConnected = await signInManager.RegisterWindowsHelloAsync();
        }

        private async Task DisableWindowsHello()
        {
            IsWindowsHelloConnected = !await signInManager.UnregisterWindowsHello();
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
                    ChangePasswordError = "Password changed successfully";
                }
                else
                {
                    ChangePasswordError = "Incorrect password";
                }
            }
            else
            {
                ChangePasswordError = "Passwords do not match";
            }
        }

        private async Task DeleteAccount()
        {
            if (await signInManager.IsWindowsHelloRegisteredAsync())
            {
                await signInManager.UnregisterWindowsHello(true);
            }
            var recordsToDelete = dbContext.GetRecordsForUser(signInManager.CurrentUserName);
            foreach (var record in recordsToDelete)
            {
                dbContext.Remove(record);
            }
            if (signInManager.Unregister(signInManager.CurrentUserName, DeleteAccountPassword))
            {
                await dbContext.SaveChangesAsync();
            }
        }

        private async void CheckWindowsHello()
        {
            IsWindowsHelloAvailable = await WindowsHelloHelper.IsWindowsHelloAvailableAsync();
            IsWindowsHelloConnected = await signInManager.IsWindowsHelloRegisteredAsync();
        }
    }
}
