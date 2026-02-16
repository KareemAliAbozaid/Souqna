using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Souqna.API.Helper;

namespace Souqna.API.Controllers
{
    [Route("erroes/{StatusCode}")]
    [ApiController]
    public class ErrorsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetError(int StatusCode)
        {
            return new ObjectResult(new ResponseApi(StatusCode));
        }
    }
}
