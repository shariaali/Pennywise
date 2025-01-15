using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pennywise.Model;

namespace Pennywise.Services.Interfaces
{
    public interface IUserService
    {
        bool Login(User user);
        string PreferredCurrency { get; }
        bool IsLoggedIn { get; }
        string LoggedInUser { get; }
        void Logout();
        
    }
}
