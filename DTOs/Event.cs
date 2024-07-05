using OutsideServer.Forms;
using OutsideServer.Models;

namespace OutsideServer.DTOs;

public class EventData : CreateEvent
{
    public required Guid Id { get; set; }
    public EventLocationData? location {  get; set; }
    public required UserData CreatedBy { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static implicit operator EventData(Event entity)
    {
        UserData createdBy = (UserData) entity.CreatedBy;
        EventLocationData? _location = 
            entity.Location is not null? (EventLocationData?) entity.Location : null;

        return new EventData
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            StartsAt = entity.StartsAt,
            FinishesAt = entity.FinishesAt,
            location = _location,
            CreatedBy = createdBy,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

public class EventLocationData : CreateEventLocation
{
    public required Guid Id { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required string State { get; set; }
    public required string City { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static implicit operator EventLocationData(EventLocation entity)
    {
        return new EventLocationData
        {
            Id = entity.Id,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            Country = entity.Country,
            State = entity.State,
            City = entity.City,
            PostalCode = entity.PostalCode,
            AddressLine1 = entity.AddressLine1,
            AddressLine2 = entity.AddressLine2,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

public class EventTicketAllotmentData : CreateEventTicketAllotment
{
    public required Guid Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static implicit operator EventTicketAllotmentData(EventTicketAllotment entity)
    {
        return new EventTicketAllotmentData
        {
            Id = entity.Id,
            Name = entity.Name,
            PaymentAmount = entity.Amount, //PaymentAmount
            TicketsQuantityLimit = entity.TicketsQuantityLimit,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
