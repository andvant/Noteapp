using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Noteapp.Core.Exceptions;
using System;
using System.Net;

namespace Noteapp.Api.Filters
{
    public class NoteExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var error = new { Error = context.Exception.Message };

            switch (context.Exception)
            {
                case NoteNotFoundException:
                    context.Result = new NotFoundObjectResult(error);
                    break;
                case NoteLockedException:
                    context.Result = new ConflictObjectResult(error);
                    break;
                case TooManyNotesException:
                    context.Result = new ObjectResult(error) { StatusCode = (int)HttpStatusCode.RequestEntityTooLarge };
                    break;
            }
        }
    }
}
