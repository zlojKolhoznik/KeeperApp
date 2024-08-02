using KeeperApp.Models;
using KeeperApp.Views.ViewFactories;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace KeeperApp.Behaviors
{
    public class ShowRecordDetailsAction : DependencyObject, IAction
    {
        public static DependencyProperty FrameProperty = DependencyProperty.Register("Frame", typeof(Frame), typeof(ShowRecordDetailsAction), new PropertyMetadata(null));
        public static DependencyProperty EnableEditingProperty = DependencyProperty.Register("EnableEditing", typeof(bool), typeof(ShowRecordDetailsAction), new PropertyMetadata(null));
        public static DependencyProperty RecordIdProperty = DependencyProperty.Register("RecordId", typeof(int), typeof(ShowRecordDetailsAction), new PropertyMetadata(null));
        public static DependencyProperty FactoryProperty = DependencyProperty.Register("Factory", typeof(IRecordViewsFactory), typeof(ShowRecordDetailsAction), new PropertyMetadata(null));

        public Frame Frame
        {
            get => (Frame)GetValue(FrameProperty);
            set => SetValue(FrameProperty, value);
        }

        public bool EnableEditing 
        {
            get => (bool)GetValue(EnableEditingProperty);
            set => SetValue(EnableEditingProperty, value);
        }

        public int RecordId
        {
            get => (int)GetValue(RecordIdProperty);
            set => SetValue(RecordIdProperty, value);
        }

        public IRecordViewsFactory Factory
        {
            get => (IRecordViewsFactory)GetValue(FactoryProperty);
            set => SetValue(FactoryProperty, value);
        }

        public object Execute(object sender, object parameter)
        {
            return Frame.Navigate(Factory.GetDetailsPageType(), new DetailsPageConfig { RecordId = RecordId, EnableEditing = EnableEditing });
        }
    }
}
