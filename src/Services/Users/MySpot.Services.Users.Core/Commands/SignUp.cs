using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Users.Core.Commands;

[Message("users", "sign_up", "users.sign_up")]
public record SignUp(Guid UserId, string Email, string Password, string? JobTitle = null, string? Role = null) : ICommand;