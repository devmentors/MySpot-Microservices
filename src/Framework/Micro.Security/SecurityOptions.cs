namespace Micro.Security;

public sealed class SecurityOptions
{
    public EncryptionOptions Encryption { get; set; } = new();

    public sealed class EncryptionOptions
    {
        public string? Key { get; set; }
    }
}