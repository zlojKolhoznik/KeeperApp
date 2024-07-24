using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeeperApp.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public class SignInViewModel : ObservableObject
    {
        private readonly SignInManager signInManager;
        private readonly ResourceLoader resourceLoader;

        private string username;
        private string password;
        private string confirmPassword;
        private string errorMessage;

        public SignInViewModel(SignInManager signInManager) 
        {
            this.signInManager = signInManager;
            resourceLoader = new ResourceLoader();
        }

        public bool IsWindowsHelloConnected => !string.IsNullOrWhiteSpace(WindowsHelloHelper.GetConnectedAccount());
        public string SignInTitle => resourceLoader.GetString("SignInNoun");
        public string RegisterTitle => resourceLoader.GetString("RegisterNoun");
        public string UsernamePlaceholder => resourceLoader.GetString("Username");
        public string PasswordPlaceholder => resourceLoader.GetString("Password");
        public string ConfirmPasswordPlaceholder => resourceLoader.GetString("ConfirmPassword");
        public string SignInVerb => resourceLoader.GetString("SignInVerb");
        public string RegisterVerb => resourceLoader.GetString("RegisterVerb");
        public string RegisterLink => resourceLoader.GetString("RegisterLink");
        public string SignInExistingAccount => resourceLoader.GetString("SignInExistingAccount");

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
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

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SignInCommand => new(SignIn);

        public RelayCommand RegisterCommand => new(Register);

        public RelayCommand WindowsHelloSignInCommand => new(WindowsHelloSignIn);

        private void SignIn()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = resourceLoader.GetString("RequiredFieldsPrompt");
            }
            else if (!signInManager.SignIn(Username, Password))
            {
                ErrorMessage = resourceLoader.GetString("InvalidSignIn");
            }
        }

        private void Register() 
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ErrorMessage = resourceLoader.GetString("RequiredFieldsPrompt");
            }
            else if (Password == ConfirmPassword)
            {
                if (!signInManager.Register(Username, Password))
                {
                    ErrorMessage = resourceLoader.GetString("InvalidRegister");
                }
            }
            else
            {
                ErrorMessage = resourceLoader.GetString("PasswordMismatch");
            }
        }

        private void WindowsHelloSignIn()
        {
            signInManager.SignInWithWindowsHello();
        }
    }
}
