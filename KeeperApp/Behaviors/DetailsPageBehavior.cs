using KeeperApp.Records;
using KeeperApp.Records.ViewAttributes;
using KeeperApp.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace KeeperApp.Behaviors
{
    internal class DetailsPageBehavior : Behavior<StackPanel>
    {
        public static DependencyProperty RecordProperty = DependencyProperty.Register("Record", typeof(Record), typeof(DetailsPageBehavior), new PropertyMetadata(null));
        public static DependencyProperty IsInEditingModeProperty = DependencyProperty.Register("IsInEditingMode", typeof(bool), typeof(DetailsPageBehavior), new PropertyMetadata(false));

        public Record Record
        {
            get => (Record)GetValue(RecordProperty);
            set
            {
                SetValue(RecordProperty, value);
                GenerateContent();
            }
        }

        public bool IsInEditingMode
        {
            get => (bool)GetValue(IsInEditingModeProperty);
            set
            {
                SetValue(IsInEditingModeProperty, value);
                GenerateContent();
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnContainerLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnContainerLoaded;
        }

        private void OnContainerLoaded(object sender, RoutedEventArgs e)
        {
            GenerateContent();
        }

        private void GenerateContent()
        {
            AssociatedObject.Children.Clear();
            if (Record is not null)
            {
                Type recordType = Record.GetType();
                var properties = recordType.GetProperties().Where(pi => pi.GetCustomAttribute<HiddenAttribute>() is null);
                foreach (var property in properties)
                {
                    AssociatedObject.Children.Add(CreateControlForProperty(property));
                }
            }
            else
            {
                AssociatedObject.Children.Add(new TextBlock { Text = "No record selected" });
            }
        }

        private Control CreateControlForProperty(PropertyInfo source)
        {
            ViewContolAttribute viewContolAttribute = source.GetCustomAttribute<ViewContolAttribute>();
            Type controlType = viewContolAttribute?.ControlType ?? typeof(TextBox);
            Control control = (Control)Activator.CreateInstance(controlType);
            PropertyInfo headerProperty = controlType.GetProperty("Header");
            headerProperty?.SetValue(control, source.Name);
            PropertyInfo readOnlyProperty = controlType.GetProperty("IsReadOnly");
            if (readOnlyProperty is not null)
            {
                readOnlyProperty.SetValue(control, !IsInEditingMode);
            }
            else if (control is PasswordBox pb && !IsInEditingMode)
            {
                ConfigureReadOnlyPasswordBox(pb, source.GetValue(Record)?.ToString() ?? "");
            }
            else
            {
                control.IsEnabled = IsInEditingMode;
            }
            PropertyInfo dpInfo = controlType.GetProperty(viewContolAttribute?.ValueDependencyPropertyName ?? "TextProperty");
            SetControlValue(control, source, (DependencyProperty)dpInfo.GetValue(control));
            control.Margin = new Thickness(10);
            return control;
        }

        private void ConfigureReadOnlyPasswordBox(PasswordBox pb, string initialPassword)
        {
            pb.PasswordChanged += (s, e) => pb.Password = initialPassword;
            pb.LostFocus += (s, e) => pb.PasswordRevealMode = PasswordRevealMode.Hidden;
            pb.GotFocus += (s, e) =>
            {
                pb.PasswordRevealMode = PasswordRevealMode.Visible;
                if (pb.FocusState == FocusState.Pointer)
                {
                    var textToCopy = new DataPackage();
                    textToCopy.SetText(pb.Password);
                    textToCopy.RequestedOperation = DataPackageOperation.Copy;
                    Clipboard.SetContent(textToCopy);
                    ShowTemporaryMessage(pb, "Password successfully copied", 2);
                }
            };
        }

        private void SetControlValue(Control control, PropertyInfo sourceProperty, DependencyProperty destinationProperty)
        {
            if (IsInEditingMode)
            {
                var binding = new Binding
                {
                    Source = Record,
                    Path = new PropertyPath(sourceProperty.Name),
                    Mode = BindingMode.TwoWay
                };
                control.SetBinding(destinationProperty, binding);
            }
            else
            {
                control.SetValue(destinationProperty, sourceProperty.GetValue(Record));
            }
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
