using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace WebApi.DependencyInjection
{
    public class DefaultDependencyScope : IDependencyScope
    {
        private readonly IServiceScope _serviceScope;

        public DefaultDependencyScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return _serviceScope.ServiceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceScope.ServiceProvider.GetServices(serviceType);
        }
    }
}