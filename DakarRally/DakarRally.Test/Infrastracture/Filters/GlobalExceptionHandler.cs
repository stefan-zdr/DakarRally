using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace DakarRally.Test.Infrastracture.Filters
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var exception = context.Exception;
            var statusCode = HttpStatusCode.InternalServerError;
            if (exception != null)
            {
                var errorMessage = exception.Message;
                var response = context.Request.CreateResponse(statusCode, new { errorMessage });
                context.Result = new ResponseMessageResult(response);
            }
            return Task.CompletedTask;
        }
    }
}