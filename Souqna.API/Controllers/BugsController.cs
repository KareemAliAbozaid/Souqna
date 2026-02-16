using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Souqna.Domin.Interfaces;

namespace Souqna.API.Controllers
{
    public class BugsController : BaseController
    {
        public BugsController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        [HttpGet]
        public IActionResult GetBugs()
        {
            try
            {
                var bugs = unitOfWork.Categories.GetAllAsync().Result;
                return Ok(bugs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetBugById(int id)
        {
            try
            {
                var bug = unitOfWork.Categories.GetByIdAsync(id).Result;
                if (bug is null)
                {
                    return NotFound(new { Message = "Bug Not Found" });
                }
                return Ok(bug);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }

    }

}
