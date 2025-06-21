using System;
using System.ComponentModel.DataAnnotations;
using Quartz;

namespace WebPageChangeMonitor.Common.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class QuartzCronExpressionAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value,
        ValidationContext validationContext)
    {
        if (value is string expression)
        {
            if (!CronExpression.IsValidExpression(expression))
            {
                return new ValidationResult("Invalid quartz cron expression format.");
            }

            return ValidationResult.Success;
        }

        return new ValidationResult("Invalid quartz cron expression value type, expected - 'string'.");
    }
}
