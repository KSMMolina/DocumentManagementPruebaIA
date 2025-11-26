using DocumentManagement.Application.Abstractions.Persistence;
using DocumentManagement.Infrastructure.Persistence;
using DocumentManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentManagement.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemory = true)
    {
        if (useInMemory)
        {
            services.AddSingleton<IFolderRepository, InMemoryFolderRepository>();
            services.AddSingleton<IDocumentFileRepository, InMemoryDocumentFileRepository>();
            services.AddSingleton<IPermissionRepository, InMemoryPermissionRepository>();
            services.AddSingleton<IAuditRepository, InMemoryAuditRepository>();
            services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
            return services;
        }

        var connectionString = configuration.GetConnectionString("DocumentManagement") ?? string.Empty;
        services.AddDbContext<DocumentDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IFolderRepository, InMemoryFolderRepository>();
        services.AddScoped<IDocumentFileRepository, InMemoryDocumentFileRepository>();
        services.AddScoped<IPermissionRepository, InMemoryPermissionRepository>();
        services.AddScoped<IAuditRepository, InMemoryAuditRepository>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        return services;
    }
}
