using System;
using System.Net;

namespace DakarRally.API.Exceptions
{
    public class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public BaseException(string errorMessage) : base(errorMessage)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }
}