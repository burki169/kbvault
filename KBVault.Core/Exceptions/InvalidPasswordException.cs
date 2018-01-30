using System;

namespace KBVault.Core.Exceptions
{
    public class InvalidPasswordException : Exception
    {
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
