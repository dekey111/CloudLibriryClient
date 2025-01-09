using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class TokenResponse
    {
        public int IdUser { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public int IdRole { get; set; }

        public string Token { get; set; }
    }
}
