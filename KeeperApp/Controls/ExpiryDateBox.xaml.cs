using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

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

        public static DependencyProperty ValueProperty { get; set; } = DependencyProperty.Register("Value", typeof(string), typeof(ExpiryDateBox), new PropertyMetadata(null));
        public static DependencyProperty HeaderProperty { get; set; } = DependencyProperty.Register("Header", typeof(string), typeof(ExpiryDateBox), new PropertyMetadata(null));
        public static DependencyProperty IsReadOnlyProperty { get; set; } = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(ExpiryDateBox), new PropertyMetadata(false));

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
                        SetValue(ValueProperty, $"{Math.Clamp(parts[0], 0, 12):D2}/{parts[1]:D2}");
                    }
                    else
                    {
                        SetValue(ValueProperty, null);
                    }
                    UpdateInputs();
                }
            }
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        private void MonthChanged(object sender, TextChangedEventArgs args)
        {
            if (int.TryParse(MonthBox.Text, out var month))
            {
                MonthBox.Text = Math.Clamp(month, 0, 12).ToString();
                MonthBox.SelectionStart = MonthBox.Text.Length;
            }
            else
            {
                MonthBox.Text = string.Empty;
            }
            UpdateValue();
        }

        private void YearChanged(object sender, TextChangedEventArgs args)
        {
            if (!int.TryParse(YearBox.Text, out var _))
            {
                YearBox.Text = string.Empty;
            }
            UpdateValue();
        }

        private void UpdateValue()
        {
            Value = $"{MonthBox.Text}/{YearBox.Text}";
        }

        private void UpdateInputs()
        {
            if (!string.IsNullOrWhiteSpace(Value))
            {
                int[] parts = Value.Split('/').Select(p => int.TryParse(p, out var val) ? val : 0).ToArray();
                MonthBox.Text = parts.Length == 2 && parts[0] != 0 ? parts[0].ToString() : string.Empty;
                YearBox.Text = parts.Length == 2 && parts[1] != 0 ? parts[1].ToString() : string.Empty;
            }
            else
            {
                MonthBox.Text = string.Empty;
                YearBox.Text = string.Empty;
            }
        }
    }
}
