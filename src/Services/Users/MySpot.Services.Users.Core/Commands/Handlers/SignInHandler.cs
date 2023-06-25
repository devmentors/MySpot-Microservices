using System.ComponentModel.DataAnnotations;
using Micro.Auth.JWT;
using Micro.Handlers;
using Micro.Security.Encryption;
using Microsoft.Extensions.Logging;
using MySpot.Services.Users.Core.Exceptions;
using MySpot.Services.Users.Core.Repositories;
using MySpot.Services.Users.Core.Services;

namespace MySpot.Services.Users.Core.Commands.Handlers;

internal sealed class SignInHandler : ICommandHandler<SignIn>
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();
    private readonly IUserRepository _userRepository;
    private readonly IJsonWebTokenManager _jsonWebTokenManager;
    private readonly IPasswordManager _passwordManager;
    private readonly ITokenStorage _tokenStorage;
    private readonly ILogger<SignInHandler> _logger;

    public SignInHandler(IUserRepository userRepository, IJsonWebTokenManager jsonWebTokenManager,
        IPasswordManager passwordManager, ITokenStorage tokenStorage, ILogger<SignInHandler> logger)
    {
        _userRepository = userRepository;
        _jsonWebTokenManager = jsonWebTokenManager;
        _passwordManager = passwordManager;
        _tokenStorage = tokenStorage;
        _logger = logger;
    }

    public async Task HandleAsync(SignIn command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || !EmailAddressAttribute.IsValid(command.Email))
        {
            throw new InvalidEmailException(command.Email);
        }
        
        if (string.IsNullOrWhiteSpace(command.Password))
        {
            throw new MissingPasswordException();
        }
        
        var user = await _userRepository.GetAsync(command.Email.ToLowerInvariant());
        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        if (!_passwordManager.IsValid(command.Password, user.Password))
        {
            throw new InvalidCredentialsException();
        }

        var claims = new Dictionary<string, IEnumerable<string>>
        {
            ["permissions"] = user.Role.Permissions
        };

        var jwt = _jsonWebTokenManager.CreateToken(user.Id.ToString(), user.Email, user.Role.Name, claims: claims);
        jwt.Email = user.Email;
        _logger.LogInformation($"User with ID: '{user.Id}' has signed in.");
        _tokenStorage.Set(jwt);
    }
}