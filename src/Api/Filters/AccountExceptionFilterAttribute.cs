using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Noteapp.Core.Exceptions;
using System;

namespace Noteapp.Api.Filters
{
    public class AccountExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var error = new { Error = context.Exception.Message };

            switch (context.Exception)
            {
                case UserRegistrationException:
                    context.Result = new BadRequestObjectResult(error);
                    break;
                case CredentialsNotValidException:
                    context.Result = new UnauthorizedObjectResult(error);
                    break;
            }
        }
    }
}
