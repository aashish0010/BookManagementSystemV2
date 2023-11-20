using Microsoft.AspNetCore.Mvc;

namespace BookManagementSystem.Controller
{
	[Route("api/[controller]")]
	[ApiController]
	public class SiteBuilderController : ControllerBase
	{
		[HttpGet]
		public IActionResult GetTags()
		{
			return Ok("");
		}
	}
}
