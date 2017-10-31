using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KBVault.Dal.Entities;

namespace KBVault.Dal.Repository
{
    public interface ISettingsRepository
    {
        Settings Get();
        void Save(Settings settings);
    }
}
