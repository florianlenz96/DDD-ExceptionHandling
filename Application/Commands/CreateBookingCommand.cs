namespace Application.Commands;

public class CreateBookingCommand
{
    public Guid CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}