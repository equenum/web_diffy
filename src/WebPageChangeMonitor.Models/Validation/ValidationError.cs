namespace WebPageChangeMonitor.Models.Validation;

// remove this record after enabling attribute based validation when dotnet 10 comes out
public record ValidationError(string PropertyName, string Message);