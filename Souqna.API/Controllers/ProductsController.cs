using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Souqna.API.Helper;
using Souqna.Application.DTOs;
using Souqna.Application.Interfaces.Repositories;
using Souqna.Domin.Sharing;

namespace Souqna.API.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        
        [HttpGet]
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
        public async Task<IActionResult> AddProduct([FromForm] AddProductDto addProductDto)
        {
            try
            {
                await unitOfWork.Products.AddAsync(addProductDto);
                return Ok(new ResponseApi(200, "Added Successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDto updateProductDto)
        {
            try
            {
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new ResponseApi(404, "Product not found."));
                }

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
