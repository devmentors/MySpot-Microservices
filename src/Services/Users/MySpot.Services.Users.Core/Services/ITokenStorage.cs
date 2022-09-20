using Micro.Auth.JWT;

namespace MySpot.Services.Users.Core.Services;

public interface ITokenStorage
{
    void Set(JsonWebToken jwt);
    JsonWebToken? Get();
}