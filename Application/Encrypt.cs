using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public abstract class Encrypt
    {
        public abstract string EncryptPassword(string key);
        public abstract string DecryptPassword(string key);
    }
}
