using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace rocketsockets
{
    public class EndpointConfiguration :
        IEndpointConfiguration
    {
        public bool AnyInterface { get; set; }
        public string BindTo { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }
        public string X509CertName { get; set; }
        public StoreName X509StoreName { get; set; }
        public StoreLocation X509StoreLocation { get; set; }

        public EndpointConfiguration( string name )
        {
            Name = name;
        }
    }
}
