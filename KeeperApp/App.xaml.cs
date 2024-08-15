using Easy_Password_Validator;
using Easy_Password_Validator.Models;
using KeeperApp.Database;
using KeeperApp.Security;
using KeeperApp.Security.Authentication;
using KeeperApp.Services;
using KeeperApp.ViewModels;
using KeeperApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Globalization;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KeeperApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public event EventHandler LanguageChanged;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Services = ConfigureServices();
            SignInManager signInManager = Services.GetService<SignInManager>();
            signInManager.UserSignedIn += SignInManager_UserSignedIn;
            signInManager.UserSignedOut += SignInManager_UserSignedOut;
            this.InitializeComponent();
        }

        public XamlRoot MainXamlRoot => m_window.Content.XamlRoot;
        public Window MainWindow => m_window;

        public string Language
        {
            get => ApplicationLanguages.PrimaryLanguageOverride;
            set
            {
                if (value != ApplicationLanguages.PrimaryLanguageOverride)
                {
                    ApplicationLanguages.PrimaryLanguageOverride = value;
                    LanguageChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void SignInManager_UserSignedOut(object sender, SignInEventArgs e)
        {
            AesEncryptor.ClearKey();
            var frame = m_window.Content as Frame;
            frame?.Navigate(typeof(AuthenticationContainerPage));
        }

        private void SignInManager_UserSignedIn(object sender, SignInEventArgs e)
        { 
            AesEncryptor.SetKey(e.Username);
            var frame = m_window.Content as Frame;
            frame?.Navigate(typeof(MainContainerPage));
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<MainViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<AddLoginViewModel>();
            services.AddTransient<AddFolderViewModel>();
            services.AddTransient<AddCardCredentialsViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<LoginInfoViewModel>();
            services.AddTransient<FolderInfoViewModel>();
            services.AddTransient<ForgotPasswordViewModel>();
            services.AddTransient<CardCredentialsInfoViewModel>();
            services.AddTransient<SignInViewModel>();
            services.AddTransient<PasswordAnalysisViewModel>();
            services.AddTransient<RecordsSerializationService>();
            services.AddTransient<IUserInteractionService, ContentDialogService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient(service => new PasswordValidatorService(new PasswordRequirements()));
            services.AddSingleton<SignInManager>();
            services.AddDbContext<KeeperDbContext>();
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            var frame = m_window.Content as Frame;
            frame?.Navigate(typeof(AuthenticationContainerPage));
            m_window.Activate();
        }

        private Window m_window;
    }
}
