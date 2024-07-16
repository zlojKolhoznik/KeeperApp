using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Authentication
{
    public class SignInEventArgs
    {
        public SignInEventArgs(string username) 
        {
            Username = username;
        }

        public string Username { get; }
    }
}
