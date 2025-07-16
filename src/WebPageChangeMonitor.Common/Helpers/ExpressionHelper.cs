using System;
using System.Linq.Expressions;

namespace WebPageChangeMonitor.Common.Helpers;

public static class ExpressionHelper
{
    public static Expression<Func<T, object>> GetPropertyLambda<T>(string propertyName) where T : class
    { 
        var parameter = Expression.Parameter(typeof(T));
        var property = Expression.Property(parameter, propertyName);

        return Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter); 
    }
}
