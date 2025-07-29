using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Validation;

namespace WebPageChangeMonitor.Api.Infrastructure.Filters;

public class UpdateResourceValidationFilter : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<Resource>().First();
        var validationContext = new ValidationContext();

        if (request.Id == Guid.Empty)
        {
            validationContext.AddError(new ValidationError(nameof(request.Id), ValidationMessages.Required));
        }

        if (request.DisplayName is null)
        {
            validationContext.AddError(new ValidationError(nameof(request.DisplayName), ValidationMessages.Required));
        }

        if (request.DisplayName is not null)
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
            {
                validationContext.AddError(new ValidationError(nameof(request.DisplayName), ValidationMessages.Empty));
            }

            if (request.DisplayName.Length > 20)
            {
                validationContext.AddError(new ValidationError(nameof(request.DisplayName), ValidationMessages.MaxLength(20)));
            }
        }

        if (request.Description is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                validationContext.AddError(new ValidationError(nameof(request.Description), ValidationMessages.Empty));
            }

            if (request.Description.Length > 50)
            {
                validationContext.AddError(new ValidationError(nameof(request.Description), ValidationMessages.MaxLength(50)));
            }
        }

        if (validationContext.HasErrors)
        {
            return TypedResults.ValidationProblem(validationContext.ToDictionary());
        }

        return await next(context);
    }
}
