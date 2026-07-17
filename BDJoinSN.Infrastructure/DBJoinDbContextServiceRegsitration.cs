
using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Infrastructure.Persistance;
using BDJoinSN.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BDJoinSN.Infrastructure
{
    public static class DBJoinDbContextServiceRegsitration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BDJoinDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("ConnectionString")));
            
            services.AddScoped<IProfileService, ProfileService>();



            return services;
        }
    }
}
