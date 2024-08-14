using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeeperApp.Security;
using KeeperApp.Security.Authentication;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public class ForgotPasswordViewModel : ObservableObject
    {
        public event EventHandler PasswordReset;

        private readonly SignInManager signInManager;
        private readonly ResourceLoader resourceLoader;
        private string username;
        private string code;
        private string newPassword;
        private string confirmPassword;
        private bool codeConfirmed;
        private string errorMessage;
        private bool confirmingCode;
        private string codeHeader;

        public ForgotPasswordViewModel(SignInManager signInManager)
        {
            this.signInManager = signInManager;
            resourceLoader = new ResourceLoader();
        }

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public string Code
        {
            get => code;
            set => SetProperty(ref code, value);
        }

        public string NewPassword
        {
            get => newPassword;
            set => SetProperty(ref newPassword, value);
        }

        public string ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }

        public bool EnteringUsername => string.IsNullOrEmpty(Username);

        public bool ConfirmingCode
        {
            get => confirmingCode;
            set => SetProperty(ref confirmingCode, value);
        }

        public bool CodeConfirmed
        {
            get => codeConfirmed;
            set => SetProperty(ref codeConfirmed, value);
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public string Confirm => resourceLoader.GetString("Confirm");
        public string ResetPasswordLabel => resourceLoader.GetString("ResetPassword");
        public string InvalidCodeLabel => resourceLoader.GetString("InvalidCode");
        public string UsernameHeader => resourceLoader.GetString("PasswordResetUsernameHeader");
        public string CodePlaceholder => resourceLoader.GetString("Code");
        public string NewPasswordLabel => resourceLoader.GetString("NewPassword");
        public string ConfirmNewPasswordLabel => resourceLoader.GetString("ConfirmNewPassword");
        public string ProceedLabel => resourceLoader.GetString("Proceed");
        public string UsernameLabel => resourceLoader.GetString("Username");
        public string CodeHeader
        {
            get => codeHeader;
            set => SetProperty(ref codeHeader, value);
        }


        public AsyncRelayCommand HandleUsernameInputCommand => new(HandleUsernameInput);

        private async Task HandleUsernameInput()
        {
            AesEncryptor.SetKey(Username);
            if (signInManager.UserExists(Username))
            {
                ErrorMessage = "";
                // We use retrieve instead of send because we want to check if there is a valid code and if not, send a new one.
                await signInManager.RetrieveValidConfirmationCodeAsync(Username);
                OnPropertyChanged(nameof(EnteringUsername));
                CodeHeader = string.Format(resourceLoader.GetString("PasswordResetCodeHeader"), signInManager.GetEmail(username), signInManager.GetCodeExpirationTime(username));
                ConfirmingCode = true;
            }
            else
            {
                ErrorMessage = resourceLoader.GetString("UserNotFound");
            }
        }

        public AsyncRelayCommand ResetPasswordCommand => new(ResetPassword);
        public AsyncRelayCommand ConfirmCodeCommand => new(ConfirmCode);

        private async Task ConfirmCode()
        {
            CodeConfirmed = await signInManager.ConfirmCodeAsync(Username, Code);
            ErrorMessage = CodeConfirmed ? "" : resourceLoader.GetString("InvalidCode");
            ConfirmingCode = !CodeConfirmed;
        }

        private async Task ResetPassword()
        {
            if (NewPassword == ConfirmPassword)
            {
                await signInManager.ResetPasswordAsync(NewPassword, Code, Username);
                PasswordReset?.Invoke(this, EventArgs.Empty);
                AesEncryptor.ClearKey();
            }
            else
            {
                ErrorMessage = resourceLoader.GetString("PasswordMismatch");
            }
        }
    }
}
