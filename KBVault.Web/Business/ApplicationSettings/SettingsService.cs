using System;
using System.Web;
using KBVault.Dal.Entities;
using KBVault.Dal.Repository;

namespace KBVault.Web.Business.ApplicationSettings
{
    public class SettingsService : ISettingsService
    {
        private const string SettingsSessionKey = "SettingsSessionKey";

        public ISettingsRepository SettingsRepository { get; set; }

        public Settings GetSettings()
        {
            if (HttpContext.Current.Session[SettingsSessionKey] == null)
            {
                ReloadSettings();
            }

            return HttpContext.Current.Session[SettingsSessionKey] as Settings;
        }

        public void ReloadSettings()
        {
            HttpContext.Current.Session[SettingsSessionKey] = SettingsRepository.Get();
        }
    }
}
