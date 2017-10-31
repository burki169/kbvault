using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBVault.Dal.Entities;

namespace KBVault.Web.Business.ApplicationSettings
{
    public interface ISettingsService
    {
        Settings GetSettings();
        void ReloadSettings();
    }
}
