using System.Runtime.CompilerServices;
using Micro.DAL.Postgres;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Services.Users.Core.DAL;
using MySpot.Services.Users.Core.DAL.Repositories;
using MySpot.Services.Users.Core.Entities;
using MySpot.Services.Users.Core.Repositories;
using MySpot.Services.Users.Core.Services;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace MySpot.Services.Users.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSingleton<ITokenStorage, HttpContextTokenStorage>()
            .AddScoped<IRoleRepository, RoleRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>()
            .AddPostgres<UsersDbContext>(configuration)
            .AddInitializer<UsersDataInitializer>();
    }
}