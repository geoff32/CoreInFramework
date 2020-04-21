using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Services.Products.Abstractions;
using WebApi.Services.Products.Models;

namespace WebApi.Services.Products
{
    internal class ProductService : IProductService
    {
        private readonly Dictionary<Guid, Product> _products;

        public ProductService()
            => _products = Enumerable.Range(1, 10)
                .Select(i => new Product($"Product {i}"))
                .ToDictionary(p => p.Id);

        public IEnumerable<Product> GetAll()
            => _products.Values.ToList();
    }
}
