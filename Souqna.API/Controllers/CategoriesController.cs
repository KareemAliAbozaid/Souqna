using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Souqna.API.Helper;
using Souqna.Application.DTOs;
using Souqna.Application.Interfaces.Repositories;

namespace Souqna.API.Controllers
{
    [Authorize]
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet]
        [Authorize(Roles = "Customer,Seller,Admin")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await unitOfWork.Categories.GetAllAsync();
                var categoryDtos = mapper.Map<IEnumerable<CategoryDto>>(categories);
                return Ok(new ResponseApiResponse<IEnumerable<CategoryDto>>(200, categoryDtos));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseApi(500, ex.Message));
            }
        }
        
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer,Seller,Admin")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new ResponseApi(404, "Category not found"));
                }
                var categoryDto = mapper.Map<CategoryDto>(category);
                return Ok(new ResponseApiResponse<CategoryDto>(200, categoryDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseApi(500, ex.Message));
            }
        }
        
        [HttpPost]
        [Authorize(Policy = "CanManageCategories")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            try
            {
                var category = mapper.Map<Souqna.Domin.Entities.Category>(categoryDto);
                await unitOfWork.Categories.AddAsync(category);
                var createdCategoryDto = mapper.Map<CategoryDto>(category);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, new ResponseApi(201));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseApi(500, ex.Message));
            }
        }
        
        [HttpPut("{id}")]
        [Authorize(Policy = "CanManageCategories")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            try
            {
                var existingCategory = await unitOfWork.Categories.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound(new ResponseApi(404, "Category not found"));
                }
                mapper.Map(updateCategoryDto, existingCategory);
                existingCategory.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.Categories.UpdateAsync(existingCategory);
               
                return Ok(new ResponseApi(200));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseApi(500, ex.Message));
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanManageCategories")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var existingCategory = await unitOfWork.Categories.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound(new ResponseApi(404, "Category not found"));
                }
                existingCategory.IsDeleted = true;
                existingCategory.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.SaveChangesAsync();
                return Ok(new ResponseApi(200, "Category deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseApi(500, ex.Message));
            }
        }
    }
}
