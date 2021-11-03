using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabulaDevApp.MVVM.Models
{
    class AuthorizationModel
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public AuthorizationModel()
        {
            Login = "";
            Password = "";
        }
    }
}
