using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TODO.AuthOptions
{
    public class AuthOptions
    {
        public const string ISSUER = "TodoApp";
        public const string AUDIENCE = "TodoAppClient";
        const string KEY = "secret6686ertywertyuiopasdfghjklmnbuiouibiuiyvucutcycc";
        public const int LIFETIME = 25;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
