using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Security
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EncryptPropertyAttribute : Attribute
    {
    }
}
