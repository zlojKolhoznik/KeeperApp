using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KeeperApp.Controls
{
    public sealed partial class ExpiryDateBox : UserControl
    {
        public ExpiryDateBox()
        {
            this.InitializeComponent();
        }

        public DependencyProperty ValueProperty { get; set; } = DependencyProperty.Register("Value", typeof(string), typeof(ExpiryDateBox), new PropertyMetadata(null));
        public DependencyProperty HeaderProperty { get; set; } = DependencyProperty.Register("Header", typeof(string), typeof(ExpiryDateBox), new PropertyMetadata(null));

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set
            {
                if (value != Value)
                {
                    int[] parts = value.Split('/').Select(p => int.TryParse(p, out var val) ? val : 0).ToArray();
                    if (parts.Length == 2 && parts.All(int.IsPositive))
                    {
                        SetValue(ValueProperty, $"{Math.Clamp(parts[0], 1, 12):D2}/{parts[1]:D2}");
                    }
                    else
                    {
                        SetValue(ValueProperty, null);
                    }
                }
            }
        }

        public string Month => Value?.Split('/')?[0] ?? string.Empty;

        public string Year => Value?.Split('/')?[1] ?? string.Empty;

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        private void MonthChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox && int.TryParse(textBox.Text, out var month))
            {
                textBox.Text = Math.Clamp(month, 0, 12).ToString();
                textBox.SelectionStart = textBox.Text.Length;
            }
            UpdateValue();
        }

        private void YearChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox && int.TryParse(textBox.Text, out var year))
            {
                textBox.Text = Math.Clamp(year, 0, 99).ToString();
                textBox.SelectionStart = textBox.Text.Length;
            }
            UpdateValue();
        }

        private void UpdateValue()
        {
            Value = $"{MonthBox.Text}/{YearBox.Text}";
        }

        private void UpdateInputs()
        {
            int[] parts = Value.Split('/').Select(p => int.TryParse(p, out var val) ? val : 0).ToArray();
            MonthBox.Text = parts.Length == 2 ? parts[0].ToString() : string.Empty;
            YearBox.Text = parts.Length == 2 ? parts[1].ToString() : string.Empty;
        }
    }
}
