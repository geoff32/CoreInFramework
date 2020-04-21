using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace WebApi.DependencyInjection
{
    public class DefaultDependencyResolver : IDependencyResolver
    {
        protected IServiceProvider ServiceProvider;


        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IDependencyScope BeginScope()
        {
            return new DefaultDependencyScope(ServiceProvider.CreateScope());
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ServiceProvider.GetServices(serviceType);
        }
    }
}