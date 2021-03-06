﻿namespace WebServer.Server.Http.Response
{
    using Enums;
    using Exceptions;
    using Server.Contracts;
    public class ViewResponse : HttpResponse
    {
        private readonly IView view;

        public ViewResponse(HttpStatusCode statusCode, IView view)
        {
            this.ValidateStatusCode(statusCode);
            this.view = view;
            this.StatusCode = statusCode;
        }

        public override string ToString()
        {
            return $"{base.ToString()}{this.view.View()}";

        }

        private void ValidateStatusCode(HttpStatusCode statusCode)
        {
            var statusCodeNumber = (int)statusCode;
            if (299 < statusCodeNumber && statusCodeNumber < 400)
            {
                throw new InvalidResponseException("View response need a status code below 300 and above 400 (inclusive).");
            }
        }
    }
}
