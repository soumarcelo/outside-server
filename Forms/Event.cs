namespace OutsideServer.Forms;

public class CreateEvent
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required DateTime StartsAt { get; set; }
    public required DateTime FinishesAt { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required string Country { get; set; }
    public required string State { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public required string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
}

public class UpdateEvent
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? FinishesAt { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
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
