namespace TaskManagmentClient.Services
{
    public class LoginState
    {
        public bool IsLoggedIn { get; private set; }
        public string Username { get; private set; }

        public event Action OnChange;

        public void SetLogin(string username)
        {
            IsLoggedIn = true;
            Username = username;
            NotifyStateChanged();
        }

        public void Logout()
        {
            IsLoggedIn = false;
            Username = string.Empty;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

