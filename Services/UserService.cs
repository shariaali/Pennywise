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
        // Property to track whether the user is logged in.
        public bool IsLoggedIn => _loggedInUser != null;
        // Property to expose the username of the logged-in user.
        public string LoggedInUser => IsLoggedIn ? _loggedInUser.Username : string.Empty;
        // Implements the Login method from IUserService.
        public string PreferredCurrency => IsLoggedIn ? _loggedInUser.PreferredCurrency : string.Empty;

        public bool Login(User user)
        {
            // Only allow login with the default username and password
            if (user.Username == DefaultUsername && user.Password == DefaultPassword)
            {
                // Create the logged-in user and store the preferred currency
                _loggedInUser = new User
                {
                    Username = user.Username,
                    Password = user.Password,
                    PreferredCurrency = user.PreferredCurrency ?? "USD" // Default to USD if no currency is provided
                };
                return true; // Login successful
            }

            _loggedInUser = null; // Reset on failed login
            return false; // Login failed
        }


        // Additional method to log out the user.
        public void Logout()
        {
            _loggedInUser = null;
        }
    }
}
