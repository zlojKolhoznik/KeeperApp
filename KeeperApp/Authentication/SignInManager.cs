using KeeperApp.Security;
using System;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace KeeperApp.Authentication
{
    public class SignInManager
    {
        public event EventHandler<SignInEventArgs> UserSignedIn;
        public event EventHandler<SignInEventArgs> UserSignedOut;

        private const string CredentialLockerResourceName = "Keeper";
        private string currentUserName;

        public SignInManager() { }

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

        public bool ChangePassword(string username, string oldPassword, string newPassword) 
        {
            bool success;
            string oldPasswordHash = Sha256Hasher.GetHash(oldPassword);
            string newPasswordHash = Sha256Hasher.GetHash(newPassword);
            try
            {
                var vault = new PasswordVault();
                var oldCredential = vault.Retrieve(CredentialLockerResourceName, username);
                oldCredential.RetrievePassword();
                success = oldCredential.Password == oldPasswordHash;
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
