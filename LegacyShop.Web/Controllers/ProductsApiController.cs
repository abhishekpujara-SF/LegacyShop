using System.Collections.Generic;
using System.Web.Http;
using LegacyShop.Web.Data;
using LegacyShop.Web.Models;

namespace LegacyShop.Web.Controllers
{
    /// <summary>Web API 2 controller: GET api/productsapi, GET api/productsapi/5, POST, DELETE.</summary>
    public class ProductsApiController : ApiController
    {
        private readonly ProductRepository _repo = new ProductRepository();

        public IEnumerable<Product> Get(int? categoryId = null)
        {
            return _repo.GetAll(categoryId);
        }

        public IHttpActionResult Get(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        public IHttpActionResult Post([FromBody] Product product)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.Name)) return BadRequest("Name is required.");
            product.ProductId = 0;
            product.ProductId = _repo.Save(product);
            return Created("api/productsapi/" + product.ProductId, product);
        }

        public IHttpActionResult Delete(int id)
        {
            _repo.Delete(id);
            return Ok();
        }
    }
}
