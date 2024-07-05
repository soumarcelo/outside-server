using OutsideServer.Forms;
using OutsideServer.Models;

namespace OutsideServer.DTOs;

public class EventData : CreateEvent
{
    public required Guid Id { get; set; }
    public required UserData CreatedBy { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static implicit operator EventData(Event entity)
    {
        UserData createdBy = (UserData) entity.CreatedBy;

        return new EventData
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            StartsAt = entity.StartsAt,
            FinishesAt = entity.FinishesAt,
            Latitude = entity.Location.Latitude,
            Longitude = entity.Location.Longitude,
            Country = entity.Location.Country,
            State = entity.Location.State,
            City = entity.Location.City,
            PostalCode = entity.Location.PostalCode,
            AddressLine1 = entity.Location.AddressLine1,
            AddressLine2 = entity.Location.AddressLine2,
            CreatedBy = createdBy,
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
