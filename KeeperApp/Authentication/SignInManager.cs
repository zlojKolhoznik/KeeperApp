using PasswordGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

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
            bool success = false;
            string passwordHash = GetHash(password);
            try
            {
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
            string passwordHash = GetHash(password);
            try
            {
                vault.Retrieve(CredentialLockerResourceName, username);
                success = false;
            }
            catch
            {
                var credential = new PasswordCredential(CredentialLockerResourceName, username, passwordHash);
                vault.Add(credential);
                CurrentUserName = username;
            }
            return success;
        }

        public bool Unregister(string username, string password) 
        {
            bool success = false;
            string passwordHash = GetHash(password);
            try
            {
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
            bool success = false;
            string oldPasswordHash = GetHash(oldPassword);
            string newPasswordHash = GetHash(newPassword);
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

        private string GetHash(string value)
        {
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hash);
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

        public async Task<bool> RegisterWindowsHelloAsync()
        {
            bool result = true;
            if (await WindowsHelloHelper.IsWindowsHelloAvailableAsync())
            {
                result = await WindowsHelloHelper.CreateKeyAsync(CurrentUserName);
            }
            return result;
        }

        public async Task<bool> UnregisterWindowsHello(bool skipConfirmation = false)
        {
            bool result = true;
            if (await IsWindowsHelloRegisteredAsync())
            {
                result = await WindowsHelloHelper.DeleteKeyAsync(CurrentUserName, skipConfirmation);
            }
            return result;
        }

        public async Task<bool> IsWindowsHelloRegisteredAsync()
        {
            return await WindowsHelloHelper.IsRegisteredForAsync(CurrentUserName);
        }
    }
}
