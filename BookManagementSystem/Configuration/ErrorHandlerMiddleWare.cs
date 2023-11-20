using BookManagementSystem.Domain.Entities;
using Newtonsoft.Json;

namespace BookManagementSystem.Configuration
{
	public class ErrorHandlerMiddleWare
	{
		private readonly RequestDelegate _requestDelegate;
		private readonly ILogger<dynamic> _logger;
		public ErrorHandlerMiddleWare(RequestDelegate request, ILogger<dynamic> logger)
		{
			_requestDelegate = request;
			_logger = logger;

		}
		public async Task InvokeAsync(HttpContext content)
		{
			try
			{
				await _requestDelegate(content);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
				content.Response.StatusCode = 500;
				content.Response.ContentType = "application/json";
				var common = new Common();
				common.Code = StatusCodes.Status400BadRequest;
				common.Message = ex.Message;
				common.Status = Level.Failed;
				await content.Response.WriteAsync(JsonConvert.SerializeObject(common));
			}
		}
	}
}
