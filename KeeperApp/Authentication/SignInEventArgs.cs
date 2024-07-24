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
