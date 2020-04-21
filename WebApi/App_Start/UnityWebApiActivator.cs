using Microsoft.Extensions.DependencyInjection;
using System.Web.Http;
using Unity.Microsoft.DependencyInjection;
using WebApi.DependencyInjection;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WebApi.UnityWebApiActivator), nameof(WebApi.UnityWebApiActivator.Start))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(WebApi.UnityWebApiActivator), nameof(WebApi.UnityWebApiActivator.Shutdown))]

namespace WebApi
{
    /// <summary>
    /// Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET.
    /// </summary>
    public static class UnityWebApiActivator
    {
        /// <summary>
        /// Integrates Unity when the application starts.
        /// </summary>
        public static void Start() 
        {
            var startup = new Startup();
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            var resolver = new DefaultDependencyResolver(UnityConfig.Container.BuildServiceProvider(services));

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        /// <summary>
        /// Disposes the Unity container when the application is shut down.
        /// </summary>
        public static void Shutdown()
        {
            UnityConfig.Container.Dispose();
        }
    }
}