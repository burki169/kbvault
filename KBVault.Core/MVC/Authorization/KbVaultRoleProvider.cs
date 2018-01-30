using System;
using System.Web.Security;
using NLog;
using KbUser = KBVault.Dal.Entities.KbUser;

namespace KBVault.Core.MVC.Authorization
{
    public class KbVaultRoleProvider : RoleProvider
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public override string ApplicationName { get; set; }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
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

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                KbUser usr = KbVaultAuthHelper.GetKbUser(username);
                if (usr == null)
                {
                    throw new ArgumentOutOfRangeException(username + " not found");
                }

                return new string[] { usr.Role };
            }
            catch (Exception ex)
            {
                log.Error(ex);
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
                log.Error(ex);
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
