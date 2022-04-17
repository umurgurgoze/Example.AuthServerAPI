using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAuthServer.Core.Dtos
{
    public class UserAppDto
    {
        public string Id { get; set; } // DbContext'te string tanımlandığı için burada da string tanımladık.
        public string UserName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }

    }
}
