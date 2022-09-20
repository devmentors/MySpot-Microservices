namespace Micro.Security.Vault;

public sealed class VaultOptions
{
    public bool Enabled { get; set; }
    public string Url { get; set; } = string.Empty;
    public AuthenticationOptions Authentication { get; set; } = new();
    public bool RevokeLeaseOnShutdown { get; set; }
    public int RenewalsInterval { get; set; }
    public KeyValueOptions KV { get; set; } = new();
    public PkiOptions PKI { get; set; } = new();
    public Dictionary<string, LeaseOptions> Lease { get; set; } = new();

    public sealed class AuthenticationOptions
    {
        public string Type { get; set; } = string.Empty;
        public TokenOptions Token { get; set; } = new();
        public UserPassOptions UserPass { get; set; } = new();

        public sealed class TokenOptions
        {
            public string Token { get; set; } = string.Empty;
        }

        public sealed class UserPassOptions
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }

    public sealed class KeyValueOptions
    {
        public bool Enabled { get; set; }
        public int EngineVersion { get; set; } = 2;
        public string MountPoint { get; set; } = "secret";
        public string Path { get; set; } = string.Empty;
        public int? Version { get; set; }
    }

    public sealed class LeaseOptions
    {
        public bool Enabled { get; set; }
        public string Type { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string MountPoint { get; set; } = string.Empty;
        public bool AutoRenewal { get; set; }
        public Dictionary<string, string> Templates { get; set; } = new();
    }

    public sealed class PkiOptions
    {
        public bool Enabled { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string MountPoint { get; set; } = string.Empty;
        public string CertificateFormat { get; set; } = string.Empty;
        public string PrivateKeyFormat { get; set; } = string.Empty;
        public string CommonName { get; set; } = string.Empty;
        public string TTL { get; set; } = string.Empty;
        public string SubjectAlternativeNames { get; set; } = string.Empty;
        public string OtherSubjectAlternativeNames { get; set; } = string.Empty;
        public bool ExcludeCommonNameFromSubjectAlternativeNames { get; set; }
        public string IPSubjectAlternativeNames { get; set; } = string.Empty;
        public string URISubjectAlternativeNames { get; set; } = string.Empty;
        public bool ImportPrivateKey { get; set; }
        public HttpHandlerOptions HttpHandler { get; set; } = new();

        public sealed class HttpHandlerOptions
        {
            public bool Enabled { get; set; }
            public string Certificate { get; set; } = string.Empty;
        }
    }
}