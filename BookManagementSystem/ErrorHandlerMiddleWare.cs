using BookManagementSystem.Domain.Entities;
using Newtonsoft.Json;

namespace BookManagementSystem
{
    public class ErrorHandlerMiddleWare
    {
        private readonly RequestDelegate _requestDelegate;
        public ErrorHandlerMiddleWare(RequestDelegate request)
        {
            _requestDelegate = request;

        }
        public async Task InvokeAsync(HttpContext content)
        {
            try
            {
                await _requestDelegate(content);
            }
            catch (Exception ex)
            {
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
