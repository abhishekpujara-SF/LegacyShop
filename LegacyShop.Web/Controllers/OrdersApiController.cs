using System.Linq;
using System.Web.Http;
using LegacyShop.Web.Data;

namespace LegacyShop.Web.Controllers
{
    /// <summary>Web API 2, read-only: GET api/ordersapi and GET api/ordersapi/5 (order with its customers).</summary>
    public class OrdersApiController : ApiController
    {
        private readonly OrderRepository _orders = new OrderRepository();
        private readonly OrderCustomerRepository _links = new OrderCustomerRepository();

        public IHttpActionResult Get()
        {
            return Ok(_orders.GetOrders());
        }

        public IHttpActionResult Get(int id)
        {
            var order = _orders.GetOrder(id);
            if (order == null) return NotFound();
            return Ok(new
            {
                order.OrderId,
                order.OrderDate,
                order.Status,
                order.Total,
                Customers = _links.GetLinksForOrder(id).Select(l => new { l.CustomerId, l.CustomerName, l.Role })
            });
        }
    }
}
