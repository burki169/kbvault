using System;
using System.Linq;
using KBVault.Dal.Entities;

namespace KBVault.Dal.Repository
{
    public class SettingsRepository : ISettingsRepository
    {
        public Settings Get()
        {
            using (var db = new KbVaultContext())
            {
                return db.Settings.FirstOrDefault();
            }
        }

        public void Save(Settings settings)
        {
            using (var db = new KbVaultContext())
            {
                var set = db.Settings.FirstOrDefault();
                if (set != null)
                {
                    db.Settings.Remove(set);
                }

                db.Settings.Add(settings);
                db.SaveChanges();
            }
        }
    }
}
