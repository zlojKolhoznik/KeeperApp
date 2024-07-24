using KeeperApp.Records.ViewAttributes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.Behaviors
{
    public class PopulateAddRecordInputsAction : DependencyObject, IAction
    {
        private readonly ResourceLoader resourceLoader;

        public DependencyProperty RecordTypeProperty = DependencyProperty.Register("RecordType", typeof(Type), typeof(PopulateAddRecordInputsAction), new PropertyMetadata(null));
        public DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(Dictionary<string, string>), typeof(PopulateAddRecordInputsAction), new PropertyMetadata(null));
        public DependencyProperty ContainerProperty = DependencyProperty.Register("Container", typeof(Panel), typeof(PopulateAddRecordInputsAction), new PropertyMetadata(null));

        public PopulateAddRecordInputsAction()
        {
            resourceLoader = new ResourceLoader();
        }

        public Type RecordType { get; set; }

        public Dictionary<string, string> Properties
        {
            get => (Dictionary<string, string>)GetValue(PropertiesProperty);
            set => SetValue(PropertiesProperty, value);
        }

        public Panel Container
        {
            get => (Panel)GetValue(ContainerProperty);
            set => SetValue(ContainerProperty, value);
        }

        public object Execute(object sender, object parameter)
        {
            var eventArgs = (SelectionChangedEventArgs)parameter;
            RecordType = (Type)((ComboBoxItem)eventArgs.AddedItems[0]).Tag;
            Container.Children.Clear();
            var propertiesToDisplay = RecordType.GetProperties().Where(p => p.GetCustomAttribute<HiddenAttribute>() == null);
            foreach (var property in propertiesToDisplay)
            {
                var viewControlAttribute = property.GetCustomAttribute<ViewControlAttribute>();
                var controlType = viewControlAttribute?.ControlType ?? typeof(TextBox);
                var hintProperty = controlType.GetProperty("PlaceholderText") ?? controlType.GetProperty("Header");
                var controlInstance = (Control)Activator.CreateInstance(controlType);
                var valueDependencyProperty = controlType.GetProperty(viewControlAttribute?.ValueDependencyPropertyName ?? "TextProperty");
                Properties.TryAdd(property.Name, string.Empty);
                var bindingPath = "Properties[" + property.Name + "]";
                // Since Properties member of this class and Properties member of AddRecordViewModel class are referring to the same object, we can set binding source to this object
                var binding = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(bindingPath),
                    Mode = BindingMode.TwoWay
                };
                controlInstance.SetBinding((DependencyProperty)valueDependencyProperty.GetValue(controlInstance), binding);
                controlInstance.Margin = new Thickness(0, 0, 0, 10);
                hintProperty?.SetValue(controlInstance, resourceLoader.GetString(property.Name));
                Container.Children.Add(controlInstance);
            }
            return null;
        }
    }
}
