using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Requests;
using WebPageChangeMonitor.Models.Validation;

namespace WebPageChangeMonitor.Api.Infrastructure.Filters;

public class CreateResourceValidationFilter : IEndpointFilter
{
    // remove this class after enabling attribute based validation when dotnet 10 comes out
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<CreateResourceRequest>().First();
        var validationContext = new ValidationContext();

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
