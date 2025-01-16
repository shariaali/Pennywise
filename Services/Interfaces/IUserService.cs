using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pennywise.Model;

namespace Pennywise.Services.Interfaces
{
    /* 
    * Interface for managing user authentication and user-specific settings
    * Provides basic user operations like login, logout, and user preferences
    */
    public interface IUserService
    {
        // Authenticates a user and returns true if login is successful
        bool Login(User user);

        // Gets the user's preferred currency setting
        string PreferredCurrency { get; }

        // Indicates whether a user is currently logged into the system
        bool IsLoggedIn { get; }

        // Returns the username of the currently logged-in user
        string LoggedInUser { get; }

        // Logs out the current user and clears the session
        void Logout();
        
    }
}
