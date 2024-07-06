namespace Domain.Exceptions;

public class DomainValidationException(string message, IDictionary<string, string[]> errors) : DomainException(message)
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}