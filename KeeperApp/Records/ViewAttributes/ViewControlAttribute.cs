using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Records.ViewAttributes
{
    /// <summary>
    /// Sets the type of the control that will be used to display the property in the details view if it needs to be other than the default TextBox
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ViewControlAttribute : Attribute
    {
        public Type ControlType { get; }
        public string ValueDependencyPropertyName { get; }

        public ViewControlAttribute(Type controlType, string valueDependencyPropertyName)
        {
            if (!controlType.IsClass || controlType.IsAbstract)
            {
                throw new ArgumentException("Control type must be a concrete class");
            }
            if (!controlType.IsSubclassOf(typeof(Microsoft.UI.Xaml.Controls.Control)))
            {
                throw new ArgumentException("Control type must be a subclass of Microsoft.UI.Xaml.Controls.Control");
            }
            ControlType = controlType;
            ValueDependencyPropertyName = valueDependencyPropertyName;
        }
    }
}
