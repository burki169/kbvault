using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBVault.Core.Exceptions
{
    public class InvalidPasswordException : Exception {
        public InvalidPasswordException()
            : base("Geçersiz Parola")
        {
        }

        public InvalidPasswordException(string msg)
            : base(msg)
        {
        } 
   }
}
