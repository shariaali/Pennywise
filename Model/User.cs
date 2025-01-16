using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pennywise.Model
{
    /* User class that stores basic user information 
     * and preferences for the Pennywise application
     */
    public class User
    {
        // The unique username for the user account
        public string Username { get; set; }

        // Stores the user's password (Note: Should be hashed in production)
        public string Password { get; set; }

        // The currency the user prefers for financial calculations
        public string PreferredCurrency { get; set; }
    }
}
