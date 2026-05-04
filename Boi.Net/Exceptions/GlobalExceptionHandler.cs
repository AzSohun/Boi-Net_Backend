using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Boi.Net.Exceptions
{
    public class GlobalExceptionHandler: IExceptionHandler
    {

        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {

            // This line of code will to show the error in to the Console Log,
            _logger.LogError(exception, "An unexpected error occured: {Message}", exception.Message);


            // Error Details
            var ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An Unexpected Server Error Occured. Boi.Net Server is Down. We Are Fixing. Try Again Later",
                Instance = httpContext.Request.Path // This will track the error source.
            };


            // For Sending Response.
            httpContext.Response.StatusCode = ProblemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(ProblemDetails, cancellationToken);


            return true;
        }
    }
}
