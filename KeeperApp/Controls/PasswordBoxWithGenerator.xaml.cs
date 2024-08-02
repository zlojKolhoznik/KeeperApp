using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PasswordGenerator;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KeeperApp.Controls
{
    public sealed partial class PasswordBoxWithGenerator : UserControl
    {
        private const int MaxGeneratingAttempts = 1000;
        private readonly ResourceLoader resourceLoader;

        private string savedPassword;

        public PasswordBoxWithGenerator()
        {
            this.InitializeComponent();
            resourceLoader = new ResourceLoader();
        }

        public static DependencyProperty PasswordProperty { get; set; } = DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxWithGenerator), new PropertyMetadata(""));
        public static DependencyProperty PasswordLengthProperty { get; set; } = DependencyProperty.Register("PasswordLength", typeof(int), typeof(PasswordBoxWithGenerator), new PropertyMetadata(8));
        public static DependencyProperty UseLowercaseProperty { get; set; } = DependencyProperty.Register("UseLowercase", typeof(bool), typeof(PasswordBoxWithGenerator), new PropertyMetadata(true));
        public static DependencyProperty UseUppercaseProperty { get; set; } = DependencyProperty.Register("UseUppercase", typeof(bool), typeof(PasswordBoxWithGenerator), new PropertyMetadata(true));
        public static DependencyProperty UseDigitsProperty { get; set; } = DependencyProperty.Register("UseDigits", typeof(bool), typeof(PasswordBoxWithGenerator), new PropertyMetadata(true));
        public static DependencyProperty UseSpecialCharactersProperty { get; set; } = DependencyProperty.Register("UseSpecialCharacters", typeof(bool), typeof(PasswordBoxWithGenerator), new PropertyMetadata(true));
        public static DependencyProperty PasswordRevealModeProperty { get; set; } = DependencyProperty.Register("PasswordRevealMode", typeof(PasswordRevealMode), typeof(PasswordBoxWithGenerator), new PropertyMetadata(PasswordRevealMode.Peek));
        public static DependencyProperty IsReadOnlyProperty { get; set; } = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(PasswordBoxWithGenerator), new PropertyMetadata(false));
        public static DependencyProperty PlaceholderTextProperty { get; set; } = DependencyProperty.Register("PlaceholderText", typeof(string), typeof(PasswordBoxWithGenerator), new PropertyMetadata(null));
        public static DependencyProperty HeaderProperty { get; set; } = DependencyProperty.Register("Header", typeof(string), typeof(PasswordBoxWithGenerator), new PropertyMetadata(null));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set
            {
                if (!IsReadOnly)
                {
                    SetValue(PasswordProperty, value);
                }
            }
        }

        public int PasswordLength
        {
            get => (int)GetValue(PasswordLengthProperty);
            set => SetValue(PasswordLengthProperty, value);
        }

        public bool UseLowercase
        {
            get => (bool)GetValue(UseLowercaseProperty);
            set => SetValue(UseLowercaseProperty, value);
        }

        public bool UseUppercase
        {
            get => (bool)GetValue(UseUppercaseProperty);
            set => SetValue(UseUppercaseProperty, value);
        }

        public bool UseDigits
        {
            get => (bool)GetValue(UseDigitsProperty);
            set => SetValue(UseDigitsProperty, value);
        }

        public bool UseSpecialCharacters
        {
            get => (bool)GetValue(UseSpecialCharactersProperty);
            set => SetValue(UseSpecialCharactersProperty, value);
        }

        public PasswordRevealMode PasswordRevealMode
        {
            get => (PasswordRevealMode)GetValue(PasswordRevealModeProperty);
            set => SetValue(PasswordRevealModeProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set  
            {
                SetValue(IsReadOnlyProperty, value);
                SetPasswordBoxReadOnlyMode(value);
            }
        }

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        private void SetPasswordBoxReadOnlyMode(bool value)
        {
            GenerateButton.IsEnabled = !value;
            if (value)
            {
                savedPassword = Password;
                PasswordBox.GotFocus += (s, e) => CopyAndRevealPassword();
                PasswordBox.PasswordChanged += PreventEditing;
            }
            else
            {
                savedPassword = null;
                PasswordBox.GotFocus -= (s, e) => CopyAndRevealPassword();
                PasswordBox.PasswordChanged -= PreventEditing;
            }
        }

        private void PreventEditing(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = savedPassword;
        }

        public void GeneratePassword()
        {
            var settings = new PasswordSettings(UseLowercase, UseUppercase, UseDigits, UseSpecialCharacters, PasswordLength, MaxGeneratingAttempts, false);
            var generator = new Password(settings);
            Password = generator.Next();
            CopyAndRevealPassword();
        }

        private void CopyAndRevealPassword()
        {
            PasswordRevealMode = PasswordRevealMode.Visible;
            var textToCopy = new DataPackage();
            textToCopy.SetText(Password);
            textToCopy.RequestedOperation = DataPackageOperation.Copy;
            Clipboard.SetContent(textToCopy);
            ShowTemporaryMessage(PasswordBox, resourceLoader.GetString("PasswordCopied"), 2);
        }

        private void HidePassword(object sender, RoutedEventArgs e)
        {
            PasswordRevealMode = PasswordRevealMode.Peek;
        }

        private static void ShowTemporaryMessage(Control targetControl, string message, int durationInSeconds)
        {
            var tooltip = new ToolTip { Content = message };
            ToolTipService.SetToolTip(targetControl, tooltip);
            tooltip.IsOpen = true;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(durationInSeconds) };
            timer.Tick += (s, e) =>
            {
                tooltip.IsOpen = false;
                ToolTipService.SetToolTip(targetControl, null);
                timer.Stop();
            };
            timer.Start();
        }
    }
}
