using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souqna.API.Helper;
using Souqna.Application.DTOs;
using Souqna.Application.Interfaces.Repositories;
using Souqna.Domin.Sharing;
using System.Security.Claims;

namespace Souqna.API.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductParams productParams)
        {
            try
            {
                var products = await unitOfWork.Products.GetAllAsync(productParams);
                var count = await unitOfWork.Products.CountAsync(); 
                var pagination = new Pagination<ProductDto>(products, productParams.PageNumber, productParams.PageSize, count);
                return Ok(pagination);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await unitOfWork.Products.GetByIdAsync(id, x => x.Category, x => x.Photos);
                if (product is null)
                {
                    return NotFound(new ResponseApi(404, "Product Not Found"));
                }
                var productDto = mapper.Map<ProductDto>(product);
                return Ok(new ResponseApiResponse<ProductDto>(200, productDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "CanManageProducts")]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDto addProductDto)
        {
            try
            {
                // set seller id from authenticated user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                addProductDto.SellerId = userId;
                await unitOfWork.Products.AddAsync(addProductDto);
                return Ok(new ResponseApi(200, "Added Successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        
        [HttpPut]
        [Authorize(Policy = "CanManageProducts")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDto updateProductDto)
        {
            try
            {
                // ownership validation: sellers can only update their own products
                var existing = await unitOfWork.Products.GetByIdAsync(updateProductDto.Id);
                if (existing == null)
                    return NotFound(new ResponseApi(404, "Product Not Found"));

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");
                if (!isAdmin && existing.SellerId != userId)
                    return Forbid();

                updateProductDto.SellerId = userId;
                var isUpdated = await unitOfWork.Products.UpdateAsync(updateProductDto);
                if (!isUpdated)
                {
                    return NotFound(new ResponseApi(404, "Product Not Found"));
                }
                return Ok(new ResponseApi(200, "Updated Successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanManageProducts")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new ResponseApi(404, "Product not found."));
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");
                if (!isAdmin && product.SellerId != userId)
                    return Forbid();

                product.IsDeleted = true;
                await unitOfWork.SaveChangesAsync();

                return Ok(new ResponseApi(200, "Product deleted successfully."));
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new ResponseApi(404, knfEx.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseApi(500, "An error occurred while deleting the product."));
            }
        }
    }
}
