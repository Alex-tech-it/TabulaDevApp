using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TabulaDevApp.MVVM.Models
{
    public class AuthorizationModel
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
