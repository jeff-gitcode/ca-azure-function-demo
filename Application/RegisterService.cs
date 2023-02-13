using System.Reflection;
using Application.Interfaces.API;
using Application.Interfaces.SPI;
using Application.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;
public static class RegisterService
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddScoped<IUserUseCase, UserUseCase>();
        return services;
    }
}
