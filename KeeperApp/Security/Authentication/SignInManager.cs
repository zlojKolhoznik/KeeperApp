using KeeperApp.Database;
using KeeperApp.Services;
using PasswordGenerator;
using System;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace KeeperApp.Security.Authentication
{
    public class SignInManager
    {
        public event EventHandler<SignInEventArgs> UserSignedIn;
        public event EventHandler<SignInEventArgs> UserSignedOut;
        public event EventHandler EmailConfirmationStatusChanged;

        private const string CredentialLockerResourceName = "Keeper";
        private readonly KeeperDbContext dbContext;
        private readonly IEmailSender emailSender;
        private string currentUserName;

        public SignInManager(KeeperDbContext dbContext, IEmailSender emailSender)
        {
            this.dbContext = dbContext;
            this.emailSender = emailSender;
        }

        public bool IsEmailConfirmed => dbContext.Users.Find(Sha256Hasher.GetHash(CurrentUserName))?.IsEmailConfirmed ?? false;

        public string CurrentUserName
        {
            get => currentUserName;
            private set
            {
                if (currentUserName != value)
                {
                    currentUserName = value;
                    (value is null ? UserSignedOut : UserSignedIn)?.Invoke(this, new SignInEventArgs(value));
                }
            }
        }

        public bool UserExists(string username)
        {
            return dbContext.Users.Find(Sha256Hasher.GetHash(username)) is not null;
        }

        public string GetEmail(string username)
        {
            return dbContext.Users.Find(Sha256Hasher.GetHash(username))?.Email;
        }

        public DateTime? GetCodeExpirationTime(string username)
        {
            return dbContext.Users.Find(Sha256Hasher.GetHash(username))?.ConfirmationCodeExpiration; 
        }

        public async Task SetEmailAsync(string email)
        {
            var userInfo = dbContext.Users.Find(Sha256Hasher.GetHash(CurrentUserName)) ?? throw new InvalidOperationException("User not found.");
            userInfo.Email = email;
            userInfo.IsEmailConfirmed = false;
            await dbContext.SaveChangesAsync();
            await SendConfirmationCodeAsync(CurrentUserName);
            EmailConfirmationStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task SendConfirmationCodeAsync(string username)
        {
            AesEncryptor.SetKey(username);
            var userInfo = dbContext.Users.Find(Sha256Hasher.GetHash(username));
            if (userInfo is null || userInfo.Email is null)
            {
                throw new InvalidOperationException("Email address is not set for the user.");
            }
            var codeGenerator = new Password(includeLowercase: false, includeUppercase: false, includeNumeric: true, includeSpecial: false, passwordLength: 8);
            string confirmationCode = codeGenerator.Next();
            userInfo.ConfirmationCode = confirmationCode;
            userInfo.ConfirmationCodeExpiration = DateTime.Now.AddHours(10);
            await dbContext.SaveChangesAsync();
            await emailSender.SendEmailAsync(userInfo.Email, "Keeper - Email Confirmation", $"Hello, {username}!\nYour confirmation code is {confirmationCode}. It is valid till {userInfo.ConfirmationCodeExpiration:dd.MM.yyyy-HH:mm:ss}");
        }

        public async Task<string> RetrieveValidConfirmationCodeAsync(string username)
        {
            var userInfo = dbContext.Users.Find(Sha256Hasher.GetHash(username)) ?? throw new InvalidOperationException("Confirmation code is not set.");
            if (userInfo.ConfirmationCodeExpiration is null || userInfo.ConfirmationCodeExpiration < DateTime.Now)
            {
                // If the confirmation code is expired or not sent yet, generate a new one.
                await SendConfirmationCodeAsync(username);
            }
            return userInfo.ConfirmationCode;
        }

        public async Task<bool> ConfirmCodeAsync(string username, string codeFromUser)
        {
            var generatedCode = await RetrieveValidConfirmationCodeAsync(username);
            return generatedCode == codeFromUser;
        }

        public async Task ConfirmEmailAsync(string codeFromUser)
        {
            if (!await ConfirmCodeAsync(CurrentUserName, codeFromUser))
            {
                throw new InvalidOperationException("Invalid confirmation code.");
            }
            var userInfo = dbContext.Users.Find(Sha256Hasher.GetHash(CurrentUserName)) ?? throw new InvalidOperationException("User not found.");
            userInfo.IsEmailConfirmed = true;
            userInfo.ConfirmationCode = null;
            userInfo.ConfirmationCodeExpiration = null;
            dbContext.SaveChanges();
            EmailConfirmationStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task ResetPasswordAsync(string newPassword, string codeFromUser, string username)
        {
            if (!await ConfirmCodeAsync(username, codeFromUser))
            {
                throw new InvalidOperationException("Invalid confirmation code.");
            }
            var userInfo = dbContext.Users.Find(Sha256Hasher.GetHash(username)) ?? throw new InvalidOperationException("User not found.");
            userInfo.ConfirmationCode = null;
            userInfo.ConfirmationCodeExpiration = null;
            dbContext.SaveChanges();
            ChangePassword(username, string.Empty, newPassword, reset: true);
        }

        public bool SignIn(string username, string password)
        {
            bool success;
            try
            {
                string passwordHash = Sha256Hasher.GetHash(password);
                var vault = new PasswordVault();
                var credential = vault.Retrieve(CredentialLockerResourceName, username);
                credential.RetrievePassword();
                success = credential.Password == passwordHash;
            }
            catch
            {
                success = false;
            }
            if (success)
            {
                CurrentUserName = username;
            }
            return success;
        }

        public bool Register(string username, string password)
        {
            bool success = true;
            var vault = new PasswordVault();
            try
            {
                vault.Retrieve(CredentialLockerResourceName, username);
                success = false;
            }
            catch
            {
                string passwordHash = Sha256Hasher.GetHash(password);
                var credential = new PasswordCredential(CredentialLockerResourceName, username, passwordHash);
                vault.Add(credential);
                CurrentUserName = username;
                dbContext.Users.Add(new UserInfo { UsernameHash = Sha256Hasher.GetHash(username) });
                dbContext.SaveChanges();
            }
            return success;
        }

        public bool Unregister(string username, string password)
        {
            bool success;
            try
            {
                string passwordHash = Sha256Hasher.GetHash(password);
                var vault = new PasswordVault();
                var credential = vault.Retrieve(CredentialLockerResourceName, username);
                credential.RetrievePassword();
                success = credential.Password == passwordHash;
                if (success)
                {
                    vault.Remove(credential);
                    CurrentUserName = null;
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword, bool reset = false)
        {
            bool success;
            string oldPasswordHash = Sha256Hasher.GetHash(oldPassword);
            string newPasswordHash = Sha256Hasher.GetHash(newPassword);
            try
            {
                var vault = new PasswordVault();
                var oldCredential = vault.Retrieve(CredentialLockerResourceName, username);
                oldCredential.RetrievePassword();
                success = oldCredential.Password == oldPasswordHash || reset;
                if (success)
                {
                    var newCredential = new PasswordCredential(CredentialLockerResourceName, username, newPasswordHash);
                    vault.Remove(oldCredential);
                    vault.Add(newCredential);
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public void SignOut()
        {
            CurrentUserName = null;
        }

        public async void SignInWithWindowsHello()
        {
            if (await WindowsHelloHelper.IsWindowsHelloAvailableAsync())
            {
                CurrentUserName = await WindowsHelloHelper.SignInAsync();
            }
        }

        public async Task<bool> ConnectWindowsHelloAsync()
        {
            bool result = true;
            if (await WindowsHelloHelper.IsWindowsHelloAvailableAsync())
            {
                result = await WindowsHelloHelper.CreateKeyAsync(CurrentUserName);
            }
            return result;
        }

        public async Task<bool> DisconnectWindowsHello(bool skipConfirmation = false)
        {
            bool result = true;
            if (await IsWindowsHelloConnectedAsync())
            {
                result = await WindowsHelloHelper.DeleteKeyAsync(CurrentUserName, skipConfirmation);
            }
            return result;
        }

        public async Task<bool> IsWindowsHelloConnectedAsync()
        {
            return await WindowsHelloHelper.IsConnectedToAsync(CurrentUserName);
        }
    }
}
