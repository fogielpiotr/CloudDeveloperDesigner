using System.Text.Json;
using System.Net;

namespace Api.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                string message;

                switch (error)
                {
                    case ArgumentException a:
                        message = a.Message;
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case UnauthorizedAccessException u: 
                        message = u.Message;
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case FluentValidation.ValidationException v:
                        message = v.Message; 
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        message = e.Message;
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        message = "Internal Server Error";
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new { message });
                await response.WriteAsync(result);
            }
        }
    }
}
