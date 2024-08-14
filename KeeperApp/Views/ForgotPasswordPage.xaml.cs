using KeeperApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KeeperApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ForgotPasswordPage : Page
    {
        public ForgotPasswordPage()
        {
            this.InitializeComponent();
            ViewModel = App.Current.Services.GetService<ForgotPasswordViewModel>();
            ViewModel.PasswordReset += ViewModel_PasswordReset;
        }

        // We use event handler to navigate because NavigateToPageAction didn't execute on this page for some reason.
        private void ViewModel_PasswordReset(object sender, EventArgs e)
        {
            Frame parent = (Frame)Parent;
            parent.Navigate(typeof(SignInPage));
        }

        public ForgotPasswordViewModel ViewModel { get; set; }
    }
}
