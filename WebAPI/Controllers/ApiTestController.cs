using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebAPI.Controllers {
	[Route("[controller]")]
	[ApiController]
	[Authorize]
	public class ApiTestController : ControllerBase {
		[HttpGet]
		public async Task<JsonResult> GetAsync() {
			return new JsonResult(new { 
				status = "OK",
				time = DateTime.UtcNow
			});
		}
	}
}
