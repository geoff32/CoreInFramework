using WebApi.Services.Products.Abstractions;
using System.Web.Http;

namespace WebApi.Products
{
    [RoutePrefix("api/products")]
    public class ProductController : ApiController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
            => _productService = productService;

        [HttpGet, Route]
        public IHttpActionResult GetAll()
            => Ok(_productService.GetAll());
    }
}
