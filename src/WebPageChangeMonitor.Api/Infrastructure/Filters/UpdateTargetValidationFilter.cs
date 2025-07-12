using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Quartz;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Validation;

namespace WebPageChangeMonitor.Api.Infrastructure.Filters;

public class UpdateTargetValidationFilter : IEndpointFilter
{
    // remove this class after enabling attribute based validation when dotnet 10 comes out
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<Target>().First();
        var validationContext = new ValidationContext();

        if (request.Id == Guid.Empty)
        {
            validationContext.AddError(new ValidationError(nameof(request.Id), ValidationMessages.Required));
        }

        if (request.ResourceId == Guid.Empty)
        {
            validationContext.AddError(new ValidationError(nameof(request.ResourceId), ValidationMessages.Required));
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

        if (request.Url is null)
        {
            validationContext.AddError(new ValidationError(nameof(request.Url), ValidationMessages.Required));
        }

        if (request.Url is not null)
        {
            var isValidUrl = Uri.TryCreate(request.Url, UriKind.Absolute, out var uriResult);

            if (!isValidUrl)
            {
                validationContext.AddError(new ValidationError(nameof(request.Url), ValidationMessages.Url));
            }

            if (isValidUrl && uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            { 
                validationContext.AddError(new ValidationError(nameof(request.Url), ValidationMessages.UrlSchema));
            }
        }

        if (request.CronSchedule is null)
        {
            validationContext.AddError(new ValidationError(nameof(request.CronSchedule), ValidationMessages.Required));
        }

        if (request.CronSchedule is not null && !CronExpression.IsValidExpression(request.CronSchedule))
        {
            validationContext.AddError(new ValidationError(nameof(request.CronSchedule), ValidationMessages.CronExpression));
        }

        if (request.HtmlTag is null)
        {
            validationContext.AddError(new ValidationError(nameof(request.HtmlTag), ValidationMessages.Required));
        }

        if (request.HtmlTag is not null)
        {
            if (string.IsNullOrWhiteSpace(request.HtmlTag))
            {
                validationContext.AddError(new ValidationError(nameof(request.HtmlTag), ValidationMessages.Empty));
            }

            if (request.HtmlTag.Length > 20)
            {
                validationContext.AddError(new ValidationError(nameof(request.HtmlTag), ValidationMessages.MaxLength(20)));
            }
        }

        if (request.SelectorValue is null)
        {
            validationContext.AddError(new ValidationError(nameof(request.SelectorValue), ValidationMessages.Required));
        }

        if (request.SelectorValue is not null)
        {
            if (string.IsNullOrWhiteSpace(request.SelectorValue))
            {
                validationContext.AddError(new ValidationError(nameof(request.SelectorValue), ValidationMessages.Empty));
            }

            if (request.SelectorValue.Length > 50)
            {
                validationContext.AddError(new ValidationError(nameof(request.SelectorValue), ValidationMessages.MaxLength(50)));
            }
        }

        if (request.ExpectedValue is not null)
        {
            if (string.IsNullOrWhiteSpace(request.ExpectedValue))
            {
                validationContext.AddError(new ValidationError(nameof(request.ExpectedValue), ValidationMessages.Empty));
            }

            if (request.ExpectedValue.Length > 100)
            {
                validationContext.AddError(new ValidationError(nameof(request.ExpectedValue), ValidationMessages.MaxLength(100)));
            }
        }

        if (validationContext.HasErrors)
        {
            return TypedResults.ValidationProblem(validationContext.ToDictionary());
        }

        return await next(context);
    }
}
