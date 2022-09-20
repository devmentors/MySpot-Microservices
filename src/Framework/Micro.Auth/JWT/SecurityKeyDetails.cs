using Microsoft.IdentityModel.Tokens;

namespace Micro.Auth.JWT;

internal sealed record SecurityKeyDetails(SecurityKey Key, string Algorithm);
