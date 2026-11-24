using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDJoinSN.Identity.Seed
{
    public class SeedData
    {
        public static async Task SeedAllAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger("Seed");

            try
            {
                logger.LogInformation("🚀 Iniciando proceso de Seed...");

                await IdentitySeedData.SeedAsync(serviceProvider, logger, numberOfUsers: 50);

                logger.LogInformation("✅ Seed completado exitosamente");
                logger.LogInformation("🔑 Contraseña de todos los usuarios: 'Password123!'");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error en SeedAll");
                throw;
            }
        }
    }
}
