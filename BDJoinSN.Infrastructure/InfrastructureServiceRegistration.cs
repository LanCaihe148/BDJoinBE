using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Identity.Models;
using BDJoinSN.Identity.Repositories;
using BDJoinSN.Infrastructure.Persistance;
using BDJoinSN.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BDJoinSN.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BDJoinDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("ConnectionString")));

            //services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUnitOfWork>(sp =>
            {
                var context = sp.GetRequiredService<BDJoinDbContext>();
                var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
                return new UnitOfWork(context, userManager);
            });


            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IProfileCreationService, ProfileCreationService>();

            return services;
        }
    }
}
