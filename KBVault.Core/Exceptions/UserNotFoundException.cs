using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBVault.Core.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
            : base("Kullanıcı bulunamadı.")
        {
        }

        public UserNotFoundException(string msg)
            : base(msg)
        {
        }
    }
}
