// Service class responsible for user authentication and management
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
        // Default credentials for authentication
        private const string DefaultUsername = "sharia";
        private const string DefaultPassword = "1234";

        // Stores the currently logged in user
        private User _loggedInUser;

        /* 
         * Properties to check login status and get user information
         * These properties are read-only and depend on _loggedInUser
         */
        public bool IsLoggedIn => _loggedInUser != null;
        public string LoggedInUser => IsLoggedIn ? _loggedInUser.Username : string.Empty;
        public string PreferredCurrency => IsLoggedIn ? _loggedInUser.PreferredCurrency : string.Empty;

    
        /// Attempts to log in a user with provided credentials
        /// Returns true if login successful, false otherwise
        
        public bool Login(User user)
        {
            // Check if credentials match the default values
            if (user.Username == DefaultUsername && user.Password == DefaultPassword)
            {
                // Create new user instance with provided details
                _loggedInUser = new User
                {
                    Username = user.Username,
                    Password = user.Password,
                    PreferredCurrency = user.PreferredCurrency ?? "USD" // Default to USD if no currency specified
                };
                return true;
            }

            // Login failed - clear current user and return false
            _loggedInUser = null;
            return false;
        }

        // Logs out the current user by clearing the _loggedInUser field
        public void Logout()
        {
            _loggedInUser = null;
        }
    }
}
