using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using WebApi.DependencyInjection;

namespace WebApi
{
    public static class DependencyInjectionBootStrapper
    {
        public static void Configure(HttpConfiguration config)
        {
            var startup = new Startup();
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddApiControllers();
            startup.ConfigureServices(services);

            config.DependencyResolver = new DefaultDependencyResolver(services.BuildServiceProvider());
        }

        private static void AddApiControllers(this IServiceCollection services)
        {
            foreach (var controllerType in typeof(DependencyInjectionBootStrapper).Assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(IHttpController).IsAssignableFrom(t)
                     && t.Name.EndsWith("Controller", StringComparison.Ordinal)))
            {
                services.AddScoped(controllerType);
            }
        }
    }
}