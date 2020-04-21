using Unity;
using WebApi.Services.Products.Abstractions;

namespace WebApi.Services.Products.DependencyInjection
{
    public static class UnityExtensions
    {
        public static void RegisterProducts(this IUnityContainer container)
        {
            container.RegisterType<IProductService, ProductService>(TypeLifetime.Singleton);
        }
    }
}
