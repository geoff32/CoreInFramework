using Microsoft.Extensions.DependencyInjection;
using WebApi.Services.Products.DependencyInjection;

namespace WebApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddProducts();
        }
    }
}