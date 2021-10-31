using System;
using Plainly.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Plainly.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseModule(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("AppDbContext");
            
            services.AddDbContext<AppDbContext>(context =>
            {
                context.UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention();
            });

            services.AddScoped<DbContext>(provider => provider.GetService<AppDbContext>());

            return services;
        }

        public static IApplicationBuilder UseApplicationDatabase(this IApplicationBuilder app,
            IServiceProvider serviceProvider, IHostEnvironment environment)
        {
            if (!environment.IsDevelopment() && !environment.IsProduction())
            {
                return app;
            }

            using var scope = serviceProvider.CreateScope();
                
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            return app;
        }
    }
    
    // public class AppDbContextFactory: IDesignTimeDbContextFactory<AppDbContext>
    // {
    //     public AppDbContext CreateDbContext(string[] args)
    //     {
    //
    //     }
    // }
}
