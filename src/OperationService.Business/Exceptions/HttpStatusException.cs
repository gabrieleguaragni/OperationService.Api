﻿using OperationService.Shared.DTO.Response;

namespace OperationService.Business.Exceptions
{
    public class HttpStatusException : Exception
    {
        public int Status { get; private set; }

        public HttpStatusException(int status, string msg) : base(msg)
        {
            Status = status;
        }

        public HttpStatusException(int status, ErrorMessageResponse? errorMessageResponse) : base(errorMessageResponse == null ? "" : errorMessageResponse.Message ?? "")
        {
            Status = status;
        }
    }
}
