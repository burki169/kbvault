using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using KBVault.Dal;
using KbUser = KBVault.Dal.Entities.KbUser;

namespace KBVault.Core.MVC.Authorization
{
    public class KbVaultRoleProvider : RoleProvider
    {
        private Logger Log = LogManager.GetCurrentClassLogger();
        private string AppName;

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                return AppName;
            }

            set
            {
                AppName = value;
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username) {
            try
            {
                KbUser usr = KbVaultAuthHelper.GetKbUser(username);
                if (usr == null)
                {
                    throw new ArgumentOutOfRangeException(username + " not found");
                }

                return new string[] { usr.Role };
            } catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                KbUser usr = KbVaultAuthHelper.GetKbUser(username);
                if (usr == null)
                {
                    return false;
                }

                return usr.Role == roleName;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            return false;
        }
    }
}
