using KeeperApp.Records;
using KeeperApp.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KeeperApp.Behaviors
{
    public class ShowRecordDetailsAction : DependencyObject, IAction
    {
        public static DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(ShowRecordDetailsAction), new PropertyMetadata(null));
        public static DependencyProperty ContainerProperty = DependencyProperty.Register("Container", typeof(Panel), typeof(ShowRecordDetailsAction), new PropertyMetadata(null));
        public static DependencyProperty EnableEditingProperty = DependencyProperty.Register("EnableEditing", typeof(bool), typeof(ShowRecordDetailsAction), new PropertyMetadata(null));

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public Panel Container
        {
            get => (Panel)GetValue(ContainerProperty);
            set => SetValue(ContainerProperty, value);
        }

        public bool EnableEditing 
        {
            get => (bool)GetValue(EnableEditingProperty);
            set => SetValue(EnableEditingProperty, value);
        }

        public object Execute(object sender, object parameter)
        {
            if (!IsValidPropertyPath(Path))
            {
                throw new FormatException("Path to the property is in wrong format");
            }

            Record record = GetRecordFrom(parameter, Path);
            var info = new RecordInfo { Record = record, IsInEditingMode = EnableEditing };
            Container.Children.Clear();
            Container.Children.Add(info);
            return null;
        }

        private Record GetRecordFrom(object source, string path)
        {
            if (source is not null)
            {
                var propertiesChain = GetPropertyNamesFromPath(path);
                object current = source;
                foreach (var propertyName in propertiesChain)
                {
                    Type currentType = current.GetType();
                    string usedName = IsIndexer(propertyName) ? propertyName[..propertyName.IndexOf('[')] : propertyName;
                    PropertyInfo property = currentType.GetProperty(usedName);
                    current = IsIndexer(propertyName) ? property.GetValue(current, [GetIndexFrom(propertyName)]) : property.GetValue(current);
                }
                if (current is Record record)
                {
                    return record;
                }
                else
                {
                    throw new ArgumentException("Path does not lead to a record", nameof(path));
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(source), "Source object is null");
            }
        }

        private bool IsValidPropertyPath(string path)
        {
            return Regex.IsMatch(path, @"^([a-zA-Z_]{1}[a-zA-Z0-9_]*(\[\d+\])?(\.|$))+");
        }

        private bool IsIndexer(string propertyName)
        {
            return Regex.IsMatch(Path, @"(\[\d+\]){1}$");
        }

        private IEnumerable<string> GetPropertyNamesFromPath(string path)
        {
            return path.Split('.');
        }

        private int GetIndexFrom(string propertyName)
        {
            if (!IsIndexer(propertyName))
            {
                throw new FormatException("Path must be an indexer");
            }

            var indexString = Path[(Path.IndexOf('[') + 1)..Path.IndexOf(']')];
            return int.Parse(indexString);
        }
    }
}
