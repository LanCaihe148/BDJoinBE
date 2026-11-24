using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Models.Identity;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDJoinSN.Identity.Seed
{
    public class IdentitySeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, ILogger logger, int numberOfUsers = 20)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

               
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

               
                var roles = new[] { "Administrator", "Operator", "AuthUser" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                        logger.LogInformation($"✅ Rol '{role}' creado");
                    }
                }

                // ============================================
                // 2. CREAR ADMIN USANDO AUTH SERVICE
                // ============================================
                try
                {
                    var adminRequest = new RegistrationRequest
                    {
                        Name = "Admin",
                        Lastname = "Sistema",
                        Username = "admin",
                        Email = "admin@bdjoinsn.com",
                        Password = "Admin123!",
                        RepeatPassword = "Admin123!"
                    };

                    await authService.Register(adminRequest);
                    logger.LogInformation("Usuario administrador creado");
                }
                catch (Exception ex)
                {
                    
                    logger.LogInformation("ℹUsuario administrador ya existe");
                }

              
                var faker = new Faker("es");
                var createdCount = 0;
                var password = "Password123!";

                // Generar usuarios únicos
                var usedEmails = new HashSet<string>();
                var usedUsernames = new HashSet<string>();

                for (int i = 0; i < numberOfUsers; i++)
                {
                    // Generar datos únicos
                    string firstName, lastName, username, email;

                    do
                    {
                        firstName = faker.Name.FirstName();
                        lastName = faker.Name.LastName();
                        username = faker.Internet.UserName(firstName, lastName).ToLower();
                        email = faker.Internet.Email(firstName, lastName).ToLower();
                    }
                    while (usedEmails.Contains(email) || usedUsernames.Contains(username));

                    usedEmails.Add(email);
                    usedUsernames.Add(username);

                    var request = new RegistrationRequest
                    {
                        Name = firstName,
                        Lastname = lastName,
                        Username = username,
                        Email = email,
                        Password = password,
                        RepeatPassword = password
                    };

                    try
                    {
                        await authService.Register(request);
                        createdCount++;

                        if (createdCount % 5 == 0)
                        {
                            logger.LogInformation($"⏳ {createdCount} usuarios creados...");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning($"⚠️ Error al crear usuario {username}: {ex.Message}");
                    }
                }

                logger.LogInformation($"✅ {createdCount} usuarios aleatorios creados (contraseña: '{password}')");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error en Identity Seed");
                throw;
            }
        }
    }
}
