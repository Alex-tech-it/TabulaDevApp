using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace TabulaDevApp.Core.Network
{
    public class Configuration
    {
        private IFirebaseConfig _fcon;
        
        public IFirebaseConfig fcon
        {
            get => _fcon;
            set
            {
                _fcon = value;
            }
        }

        public Configuration()
        {
            fcon = new FirebaseConfig()
            {
                AuthSecret = "tQQXw9FD3Z75I2o9O9JesG63xanma9dg07XvB0f5",
                BasePath = "https://kanbanboard-bec26-default-rtdb.firebaseio.com/"
            };
        }

        public void UpDate()
        {
            fcon = new FirebaseConfig()
            {
                AuthSecret = "tQQXw9FD3Z75I2o9O9JesG63xanma9dg07XvB0f5",
                BasePath = "https://kanbanboard-bec26-default-rtdb.firebaseio.com/"
            };
        }
    }
}
