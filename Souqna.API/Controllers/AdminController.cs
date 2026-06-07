using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Souqna.Application.Interfaces.Repositories;
using System.Threading.Tasks;

namespace Souqna.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        public AdminController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            // TODO: Implement user listing
            return Ok(new { Message = "Get users - not implemented" });
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports()
        {
            // TODO: Implement reports
            return Ok(new { Message = "Get reports - not implemented" });
        }

        [HttpPost("manage")] 
        public async Task<IActionResult> FullManagement()
        {
            // TODO: Implement full management actions
            return Ok(new { Message = "Full management - not implemented" });
        }
    }
}
