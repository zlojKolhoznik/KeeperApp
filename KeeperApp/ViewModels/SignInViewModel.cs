using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeeperApp.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class SignInViewModel : ObservableObject
    {
        private readonly SignInManager signInManager;

        private string username;
        private string password;
        private string confirmPassword;
        private string errorMessage;

        public SignInViewModel(SignInManager signInManager) 
        {
            this.signInManager = signInManager;
        }

        public bool IsWindowsHelloConnected => !string.IsNullOrWhiteSpace(WindowsHelloHelper.GetConnectedAccount());

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
                ErrorMessage = "All credentials are required";
            }
            else if (!signInManager.SignIn(Username, Password))
            {
                ErrorMessage = "Invalid sign in attempt. Check your username and password";
            }
        }

        private void Register() 
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ErrorMessage = "All credentials are required";
            }
            else if (Password == ConfirmPassword)
            {
                if (!signInManager.Register(Username, Password))
                {
                    ErrorMessage = "There is already an account with this username. Login to existing account or try another username";
                }
            }
            else
            {
                ErrorMessage = "Passwords are not the same";
            }
        }

        private void WindowsHelloSignIn()
        {
            signInManager.SignInWithWindowsHello();
        }
    }
}
