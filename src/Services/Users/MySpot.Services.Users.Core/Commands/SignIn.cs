using Micro.Abstractions;

namespace MySpot.Services.Users.Core.Commands;

public record SignIn(string Email, string Password) : ICommand;