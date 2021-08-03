using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class UserService : IUser
    {
        readonly Context db;
        public UserService(Context db)
        {
            this.db = db;
        }
        public string check(string username, string password)
        {
            //try
            //{

            if (db.User.Any(w => w.username == username))
            {
                var user = db.User.FirstOrDefault(w => w.username == username);
                if (user.password == password)
                {
                    return "ok";
                }
                return "رمز عبور اشتباه است.";
            }
            else if (db.User.Any(w => w.email == username))
            {
                var user = db.User.FirstOrDefault(w => w.email == username);
                if (user.password == password)
                {
                    return "ok";
                }
                return "رمز عبور اشتباه است.";
            }
            return "کاربری یافت نشد";
            //}
            //catch (Exception)
            //{

            //    return null;
            //}
        }

        public bool emailexist(string email)
        {
            try
            {
                if (db.User.Any(w => w.email == email))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool usernameexist(string username)
        {
            try
            {
                if (db.User.Any(w => w.username == username))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public User whichuser(string username, string password)
        {
            try
            {

                if (db.User.Any(w => w.username == username))
                {
                    var user = db.User.FirstOrDefault(w => w.username == username);
                    if (user.password == password)
                    {
                        return user;
                    }
                }
                else if (db.User.Any(w => w.email == username))
                {
                    var user = db.User.FirstOrDefault(w => w.email == username);
                    if (user.password == password)
                    {
                        return user;
                    }
                }
                return null;
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
