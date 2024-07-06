using Domain.Exceptions;
using FluentValidation.Results;

namespace Application.Extensions;

public static class ValidationResultErrorsExtensions
{
    public static void EnsureValidDomain(this ValidationResult validationResult)
    {
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).ToArray());
            throw new DomainValidationException("Invalid request parameters.", errors);
        }
    }
}