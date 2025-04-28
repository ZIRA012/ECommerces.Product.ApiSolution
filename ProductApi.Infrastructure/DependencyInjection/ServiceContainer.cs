
using ECommmerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            //Agregamos la conecexion a la base de datos
            //agregamos el sistema de autenticacion
            SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FileNames"]!);

            //con cada peticion se crear un instancia que su tiempo de vida es igual al de una petición
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastucturePolicy (this IApplicationBuilder app)
        {
            //registramos los middlewares
            //excepcions globales, para manejar errores extremos
            // Agregamos a solo escuhar a la Gateway API: Bloque extenciosn de afuera;
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
