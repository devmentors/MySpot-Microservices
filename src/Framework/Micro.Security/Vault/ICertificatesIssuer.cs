using System.Security.Cryptography.X509Certificates;

namespace Micro.Security.Vault;

public interface ICertificatesIssuer
{
    Task<X509Certificate2?> IssueAsync();
}