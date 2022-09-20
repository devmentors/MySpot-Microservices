using Microsoft.AspNetCore.Identity;

namespace Micro.Security.Encryption;

internal sealed class PasswordManager : IPasswordManager
{
    private readonly IPasswordHasher<object> _passwordHasher;

    public PasswordManager(IPasswordHasher<object> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string Secure(string password) => _passwordHasher.HashPassword(new object(), password);

    public bool IsValid(string password, string securedPassword)
        => _passwordHasher.VerifyHashedPassword(new object(), securedPassword, password) ==
           PasswordVerificationResult.Success;
}