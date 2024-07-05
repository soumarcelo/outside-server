namespace OutsideServer.Forms;

public class CreateEvent
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required DateTime StartsAt { get; set; }
    public required DateTime FinishesAt { get; set; }
}

public class UpdateEvent
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? FinishesAt { get; set; }
}

public class CreateEventLocation
{
    public required string Country { get; set; }
    public required string PostalCode { get; set; }
    public required string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
}

public class UpdateEventLocation
{
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
}

public class CreateEventTicketAllotment
{
    public required string Name { get; set; }
    public required int PaymentAmount { get; set; }
    public required int TicketsQuantityLimit { get; set; }
}

public class UpdateEventTicketAllotment
{
    public required string Name { get; set; }
    public required int PaymentAmount { get; set; }
    public required int TicketsQuantityLimit { get; set; }
}
