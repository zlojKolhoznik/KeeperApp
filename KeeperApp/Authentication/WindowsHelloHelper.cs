using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;

namespace KeeperApp.Authentication
{
    public static class WindowsHelloHelper
    {
        private static string windowsHelloAccountFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KeeperApp", "winh.txt");

        static WindowsHelloHelper()
        {
            var dir = Path.GetDirectoryName(windowsHelloAccountFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static async Task<bool> IsWindowsHelloAvailableAsync()
        {
            return await KeyCredentialManager.IsSupportedAsync();
        }

        public static async Task<bool> CreateKeyAsync(string username)
        {
            KeyCredentialRetrievalResult result = await KeyCredentialManager.RequestCreateAsync(username, KeyCredentialCreationOption.ReplaceExisting);
            bool success = result.Status == KeyCredentialStatus.Success;
            if (success)
            {
                SaveAccount(username);
            }
            return success;
        }

        private static void SaveAccount(string username)
        {
            File.WriteAllText(windowsHelloAccountFilePath, username);
        }

        public static async Task<string> SignInAsync()
        {
            string username = GetConnectedAccount();
            if (string.IsNullOrEmpty(username))
            {
                throw new Exception("No account associated with Windows Hello");
            }

            KeyCredentialRetrievalResult result = await KeyCredentialManager.OpenAsync(username);
            if (result.Status == KeyCredentialStatus.Success)
            {
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync($"{username}, enter your PIN to confirm it is you");
                if (!consentResult.Equals(UserConsentVerificationResult.Verified))
                {
                    username = null;
                }
            }
            return username;
        }

        public static string GetConnectedAccount() 
        {
            string username;
            try
            {
                username = File.ReadAllText(windowsHelloAccountFilePath); 
            } 
            catch (IOException)
            {
                username = null;
            }
            return username;
        }

        public static async Task<bool> DeleteKeyAsync(string username, bool skipConfirmation = false)
        {
            bool keyExists = (await KeyCredentialManager.OpenAsync(username)).Status == KeyCredentialStatus.Success;
            bool confirmedByUser = skipConfirmation 
                || (await UserConsentVerifier.RequestVerificationAsync("Are you sure you want to delete your Windows Hello account?")).Equals(UserConsentVerificationResult.Verified);
            bool success = keyExists && confirmedByUser;
            if (success)
            {
                await KeyCredentialManager.DeleteAsync(username);
                File.Delete(windowsHelloAccountFilePath);
            }
            return success;
        }

        public static async Task<bool> IsRegisteredForAsync(string username)
        {
            KeyCredentialRetrievalResult result = await KeyCredentialManager.OpenAsync(username);
            return result.Status == KeyCredentialStatus.Success;
        }
    }
}
