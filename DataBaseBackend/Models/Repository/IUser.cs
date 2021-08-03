using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
   public interface IUser
    {
        string check(string username,string password);

        User whichuser(string username, string password);

        bool emailexist(string email);
        bool usernameexist(string username);
    }
}
