using System.Security.Cryptography.X509Certificates;

namespace rocketsockets
{
    public interface IEndpointConfiguration
    {
        string[] BindTo { get; set; }
        string Name { get; set; }
        int Port { get; set; }
        bool SSL { get; set; }
        string X509CertName { get; set; }
        StoreName X509StoreName { get; set; }
        StoreLocation X509StoreLocation { get; set; }
    }
}