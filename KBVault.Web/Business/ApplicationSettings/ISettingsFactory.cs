using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KBVault.Dal.Entities;
using KBVault.Web.Models;

namespace KBVault.Web.Business.ApplicationSettings
{
    public interface ISettingsFactory
    {
        SettingsViewModel CreateViewModel(Settings settings);
        Settings CreateModel(SettingsViewModel settings);
    }
}
