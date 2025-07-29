using System.Collections.Generic;
using System.Linq;

namespace WebPageChangeMonitor.Models.Validation;

public class ValidationContext
{
    private List<ValidationError> _errors = [];

    public bool HasErrors => _errors.Count > 0;

    public void AddError(ValidationError error)
    {
        _errors.Add(error);
    }

    public Dictionary<string, string[]> ToDictionary() =>
        _errors.GroupBy(error => error.PropertyName)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Select(value => value.Message).ToArray());
}
