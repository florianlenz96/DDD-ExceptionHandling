using Application.Commands;
using Domain.Exceptions;

namespace Application.Handler;

public class CreateBookingHandler
{
    public void Handle(CreateBookingCommand command)
    {
        if (command.CustomerId.ToString().StartsWith("111"))
        {
            throw new BusinessRuleValidationException("Kunden mit der ID 111 sind nicht erlaubt.");
        }
        
        if (command.CustomerId.ToString().StartsWith("999")) 
        {
            throw new TimeoutException("Das System ist momentan nicht verfügbar. Bitte versuchen Sie es später erneut.");
        }
    }
}