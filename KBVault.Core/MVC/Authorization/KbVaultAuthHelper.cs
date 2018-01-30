using System;
using System.Linq;
using System.Text;
using KBVault.Core.Exceptions;
using KBVault.Core.Outlib;
using KBVault.Dal;
using KBVault.Dal.Entities;
using NLog;

namespace KBVault.Core.MVC.Authorization
{
    public class KbVaultAuthHelper
    {
        public const string HashAlgoritm = "SHA1";
        public static readonly string RoleAdmin = "Admin";
        public static readonly string RoleManager = "Manager";
        public static readonly string RoleEditor = "Editor";

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static KbUser GetKbUser(string userName)
        {
            using (var db = new KbVaultContext())
            {
                return db.KbUsers.FirstOrDefault<KbUser>(ku => ku.UserName == userName);
            }
        }

        public static KbUser CreateUser(string username, string password, string email, string role, long author)
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    KbUser usr = new KbUser();
                    usr.Password = HashPassword(password, Guid.NewGuid().ToString().Replace("-", string.Empty));
                    usr.UserName = username;
                    usr.Email = email;
                    usr.Role = role;
                    usr.Author = author;
                    db.KbUsers.Add(usr);
                    db.SaveChanges();
                    return usr;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static string HashPassword(string password, string salt)
        {
            try
            {
                return ObviexSimpleHash.ComputeHash(password, HashAlgoritm, Encoding.Default.GetBytes(salt));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static bool ValidateUser(string userName, string password)
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    KbUser usr = GetKbUser(userName);
                    if (usr == null)
                    {
                        return false;
                    }

                    return VerifyHash(password, usr.Password);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                if (ValidateUser(username, oldPassword))
                {
                    using (var db = new KbVaultContext())
                    {
                        KbUser usr = db.KbUsers.FirstOrDefault(ku => ku.UserName == username);
                        if (usr != null)
                        {
                            usr.Password = HashPassword(newPassword, Guid.NewGuid().ToString().Replace("-", string.Empty));
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new UserNotFoundException();
                        }
                    }
                }
                else
                {
                    throw new InvalidPasswordException();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        private static bool VerifyHash(string password, string passwordHash)
        {
            return ObviexSimpleHash.VerifyHash(password, HashAlgoritm, passwordHash);
        }
    }
}
