namespace Micro.Security.Vault;

internal sealed class VaultException : Exception
{
    public VaultException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}