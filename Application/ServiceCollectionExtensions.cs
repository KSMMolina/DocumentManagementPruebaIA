using DocumentManagement.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentManagement.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<FolderUseCaseService>();
        return services;
    }
}
