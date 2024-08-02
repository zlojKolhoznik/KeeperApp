using KeeperApp.Records;
using KeeperApp.Views.ViewFactories;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Converters
{
    /// <summary>
    /// Converts a Record to the appropriate IRecordViewsFactory based on its type. ConvertBack is not implemented.
    /// </summary>
    public class RecordTypeToViewFactoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not Record)
            {
                throw new ArgumentException("Value must be a Record");
            }
            IRecordViewsFactory factory = null;
            if (value is LoginRecord)
            {
                factory = new LoginRecordViewsFactory();
            }
            else if (value is CardCredentialsRecord)
            {
                factory = new CardCredentialsRecordViewsFactory();
            }
            return factory;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
