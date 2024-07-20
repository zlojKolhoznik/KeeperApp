using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Records.ViewAttributes
{
    /// <summary>
    /// Indicates that the property should not be displayed in the details view
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HiddenAttribute : Attribute
    {
    }
}
