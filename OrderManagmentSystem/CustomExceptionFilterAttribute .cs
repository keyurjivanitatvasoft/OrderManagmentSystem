using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace OrderManagmentSystem
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new { error = context.Exception.Message };

            var json = JsonConvert.SerializeObject(response);
            context.Result = new ContentResult
            {
                Content = json,
                StatusCode = (int)HttpStatusCode.InternalServerError,
                ContentType = "application/json"
            };
        }
    }
}
