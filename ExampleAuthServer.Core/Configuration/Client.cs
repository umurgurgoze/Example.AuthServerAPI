using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAuthServer.Core.Configuration
{
    public class Client // Kendi iç projemizde kullanacağımız için model ya da dto oluşturmadık.
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public List<String> Audiences { get; set; } //Api metotlara ulaşabilecek siteleri kontrol edecek.Payload içinden.
    }
}
