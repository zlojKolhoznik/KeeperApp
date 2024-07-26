using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.ApplicationModel.Resources;
using PasswordGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KeeperApp.Controls
{
    public sealed partial class CvvBox : UserControl
    {
        private readonly ResourceLoader resourceLoader;
        private string savedCvv;

        public CvvBox()
        {
            this.InitializeComponent();
            resourceLoader = new ResourceLoader();
        }

        public static DependencyProperty CvvProperty { get; set; } = DependencyProperty.Register("Cvv", typeof(string), typeof(CvvBox), new PropertyMetadata(string.Empty));
        public static DependencyProperty PlaceholderTextProperty { get; set; } = DependencyProperty.Register("PlaceholderText", typeof(string), typeof(CvvBox), new PropertyMetadata(string.Empty));
        public static DependencyProperty IsReadOnlyProperty { get; set; } = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(CvvBox), new PropertyMetadata(false));
        public static DependencyProperty HeaderProperty { get; set; } = DependencyProperty.Register("Header", typeof(string), typeof(CvvBox), new PropertyMetadata(null));

        public string Cvv
        {
            get => (string)GetValue(CvvProperty);
            set => SetValue(CvvProperty, value);
        }

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
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

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        private void SetPasswordBoxReadOnlyMode(bool value)
        {
            if (value)
            {
                savedCvv = Cvv;
                PasswordBox.GotFocus += (s, e) => CopyAndRevealCvv();
                PasswordBox.PasswordChanged += PreventEditing;
            }
            else
            {
                savedCvv = null;
                PasswordBox.GotFocus -= (s, e) => CopyAndRevealCvv();
                PasswordBox.PasswordChanged -= PreventEditing;
            }
        }

        private void PreventEditing(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = savedCvv;
        }

        private void CopyAndRevealCvv()
        {
            PasswordBox.PasswordRevealMode = PasswordRevealMode.Visible;
            var textToCopy = new DataPackage();
            textToCopy.SetText(Cvv);
            textToCopy.RequestedOperation = DataPackageOperation.Copy;
            Clipboard.SetContent(textToCopy);
            ShowTemporaryMessage(PasswordBox, resourceLoader.GetString("CvvCopied"), 2);
        }

        private void HidePassword(object sender, RoutedEventArgs e)
        {
            PasswordBox.PasswordRevealMode = PasswordRevealMode.Peek;
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

        private void CheckCvv(object sender, RoutedEventArgs args)
        {
            if (Cvv.Any(c => !char.IsDigit(c)))
            {
                Cvv = string.Join(null, Cvv.Where(char.IsDigit));
            }
        }
    }
}
