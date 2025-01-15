using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pennywise.Model;
using Pennywise.Services.Interfaces;

namespace Pennywise.Services
{
    public class UserService : IUserService
    {
        private const string DefaultUsername = "sharia";
        private const string DefaultPassword = "1234";

        private User _loggedInUser;
        public bool IsLoggedIn => _loggedInUser != null;
        public string LoggedInUser => IsLoggedIn ? _loggedInUser.Username : string.Empty;
        public string PreferredCurrency => IsLoggedIn ? _loggedInUser.PreferredCurrency : string.Empty;

        public bool Login(User user)
        {
            if (user.Username == DefaultUsername && user.Password == DefaultPassword)
            {
                _loggedInUser = new User
                {
                    Username = user.Username,
                    Password = user.Password,
                    PreferredCurrency = user.PreferredCurrency ?? "USD"
                };
                return true;
            }

            _loggedInUser = null;
            return false;
        }

        public void Logout()
        {
            _loggedInUser = null;
        }
    }
}
