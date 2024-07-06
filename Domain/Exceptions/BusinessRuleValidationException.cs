namespace Domain.Exceptions;

public class BusinessRuleValidationException(string message) : DomainException(message);