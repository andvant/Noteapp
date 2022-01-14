using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Noteapp.Api.Dtos;
using Noteapp.Core.Exceptions;
using System.Net;

namespace Noteapp.Api.Filters
{
    public class ServiceExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var error = new ErrorResponse(context.Exception.Message);

            switch (context.Exception)
            {
                case NoteNotFoundException or SnapshotNotFoundException:
                    context.Result = new NotFoundObjectResult(error);
                    break;
                case NoteLockedException:
                    context.Result = new ConflictObjectResult(error);
                    break;
                case TooManyNotesException or TextTooLongException:
                    context.Result = new ObjectResult(error) { StatusCode = (int)HttpStatusCode.RequestEntityTooLarge };
                    break;
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
