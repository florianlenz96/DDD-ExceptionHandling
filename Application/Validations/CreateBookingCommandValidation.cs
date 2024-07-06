using Application.Commands;
using FluentValidation;

namespace Application.Validations;

public class CreateBookingCommandValidation : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidation()
    {
        this.ClassLevelCascadeMode = CascadeMode.Continue;
        this.RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.")
            .NotEqual(Guid.Empty).WithMessage("CustomerId must not be default GUID.");
        
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate is required.")
            .GreaterThan(DateTime.Now).WithMessage("StartDate must be in the future.");
        
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("EndDate is required.")
            .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");
    }
}