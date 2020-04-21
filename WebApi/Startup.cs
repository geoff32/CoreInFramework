using Microsoft.Extensions.DependencyInjection;
using WebApi.Services.Products.DependencyInjection;
using WebApi.Services.Products.Options;

namespace WebApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ProductOptions>(o => o.NbProducts = 5);
            services.AddProducts();
        }
    }
}