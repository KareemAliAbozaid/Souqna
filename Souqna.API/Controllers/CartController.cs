using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Souqna.Application.Interfaces.Repositories;

namespace Souqna.API.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : BaseController
    {
        public CartController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            // TODO: Implement retrieving customer's cart
            return Ok(new { Message = "Get customer cart - not implemented" });
        }

        [HttpPost("items")]
        public IActionResult AddItemToCart()
        {
            // TODO: Implement add item to cart
            return Ok(new { Message = "Add item to cart - not implemented" });
        }

        [HttpDelete("items/{id}")]
        public IActionResult RemoveItemFromCart(int id)
        {
            // TODO: Implement remove item from cart
            return Ok(new { Message = "Remove item from cart - not implemented" });
        }

        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            // TODO: Implement checkout
            return Ok(new { Message = "Checkout - not implemented" });
        }
    }
}
