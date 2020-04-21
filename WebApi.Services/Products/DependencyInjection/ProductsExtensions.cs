using Microsoft.Extensions.DependencyInjection;
using WebApi.Services.Products.Abstractions;

namespace WebApi.Services.Products.DependencyInjection
{
    public static class ProductsExtensions
    {
        public static void AddProducts(this IServiceCollection services)
        {
            services.AddSingleton<IProductService, ProductService>();
        }
    }
}
