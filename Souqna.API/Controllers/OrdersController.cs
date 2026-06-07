using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Souqna.Application.Interfaces.Repositories;
using System.Threading.Tasks;

namespace Souqna.API.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrdersController : BaseController
    {
        public OrdersController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            // TODO: Implement fetching orders for the authenticated customer
            return Ok(new { Message = "Get customer orders - not implemented" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            // TODO: Implement retrieving order by id for authenticated customer
            return Ok(new { Message = "Get order by id - not implemented" });
        }
    }
}
