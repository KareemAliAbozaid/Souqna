using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Souqna.Application.Interfaces.Repositories;
using System.Threading.Tasks;

namespace Souqna.API.Controllers
{
    [Authorize(Roles = "Seller")]
    public class SellerController : BaseController
    {
        public SellerController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetMyProducts()
        {
            // TODO: Implement retrieving products for the authenticated seller
            return Ok(new { Message = "Get seller products - not implemented" });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetSellerOrders()
        {
            // TODO: Implement retrieving orders for seller's products
            return Ok(new { Message = "Get seller orders - not implemented" });
        }
    }
}
