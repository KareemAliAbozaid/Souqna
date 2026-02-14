using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Souqna.API.Helper;
using Souqna.Domin.DTOs;
using Souqna.Domin.Interfaces;

namespace Souqna.API.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await unitOfWork.Products.GetAllAsync(x => x.Category, x => x.Photos);
                if (products is null)
                {
                    return NotFound(new ResponseApi(404, "Product Not Found"));
                }
                var productDto = mapper.Map<IEnumerable<ProductDto>>(products);
                return Ok(new ResponseApiResponse<IEnumerable<ProductDto>>(200, productDto));
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
                return Ok(new ResponseApi(200, "Added Succefully"));
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
                return Ok(new ResponseApi(200, "Updated Succefully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        [HttpDelete]
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
