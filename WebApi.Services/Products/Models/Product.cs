using System;

namespace WebApi.Services.Products.Models
{
    public class Product
    {
        public Product(string name, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
}
