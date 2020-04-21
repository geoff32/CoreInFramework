using System.Collections.Generic;
using WebApi.Services.Products.Models;

namespace WebApi.Services.Products.Abstractions
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
    }
}
