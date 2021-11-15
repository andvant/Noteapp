using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Noteapp.Api.Exceptions;
using System;

namespace Noteapp.Api.Filters
{
    public class NoteExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case NoteNotFoundException:
                    context.Result = new NotFoundObjectResult(context.Exception.Message);
                    break;
                case NoteLockedException:
                    context.Result = new ConflictObjectResult(context.Exception.Message);
                    break;
            }
        }
    }
}
