using KeeperApp.Authentication;
using KeeperApp.Database;
using KeeperApp.Records;
using KeeperApp.Security;
using KeeperApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KeeperApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
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

        private void SignInManager_UserSignedOut(object sender, SignInEventArgs e)
        {
            AesEncryptor.UnconfigureKey();
            var currentWindow = m_window;
            m_window = new SignInWindow();
            m_window.Activate();
            currentWindow.Close();
        }

        private void SignInManager_UserSignedIn(object sender, SignInEventArgs e)
        { 
            AesEncryptor.ConfigureKey(e.Username);
            var currentWindow = m_window;
            m_window = new MainWindow();
            m_window.Activate();
            currentWindow.Close();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<MainViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<AddRecordViewModel>();
            services.AddTransient<EditRecordViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<RecordInfoViewModel>();
            services.AddTransient<SignInViewModel>();
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
            m_window = new SignInWindow();
            m_window.Activate();
        }

        private Window m_window;
    }
}
