using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.Services
{
    internal class ContentDialogService : IUserInteractionService
    {
        private readonly ResourceLoader resourceLoader;

        public ContentDialogService()
        {
            resourceLoader = new ResourceLoader();
        }

        public async Task<bool> AskUserYesNo(string message)
        {
            var cd = new ContentDialog
            {
                Title = "KeeperApp",
                Content = message,
                PrimaryButtonText = resourceLoader.GetString("Yes"),
                CloseButtonText = resourceLoader.GetString("No"),
                XamlRoot = App.Current.MainXamlRoot
            };
            var result = await cd.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
