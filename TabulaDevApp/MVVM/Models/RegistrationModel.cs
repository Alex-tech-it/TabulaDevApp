using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabulaDevApp.MVVM.Models
{
    public class RegistrationModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordVerification { get; set; }

        public RegistrationModel()
        {
            Email = "";
            Username = "";
            Password = "";
            PasswordVerification = "";
        }
    }
}
