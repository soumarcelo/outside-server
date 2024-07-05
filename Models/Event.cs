using OutsideServer.DTOs;
using OutsideServer.Forms;
using OutsideServer.Utils;
using System.ComponentModel.DataAnnotations;

namespace OutsideServer.Models;

public class Event
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(24, MinimumLength = 3)]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public DateTime StartsAt { get; set; }

    [Required]
    public DateTime FinishesAt { get; set; }

    public EventLocation? Location { get; set; }

    public List<EventTicketAllotment>? TicketAllotments { get; set; }

    [Required]
    public UserProfile CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Event() { }

    public Event(CreateEvent form, UserProfile owner)
    {
        Name = form.Name;
        Description = form.Description;
        StartsAt = form.StartsAt;
        FinishesAt = form.FinishesAt;
        CreatedBy = owner;
    }

    public Event? Update(UpdateEvent form)
    {
        bool updated = false;
        if (Comparator.IsValidUpdate(Name, form.Name))
        {
            Name = form.Name;
            updated = true;
        }
        if (Comparator.IsValidUpdate(Description, form.Description))
        {
            Description = form.Description;
            updated = true;
        }
        if (Comparator.IsValidUpdate(StartsAt, form.StartsAt))
        {
            StartsAt = (DateTime)form.StartsAt;
            updated = true;
        }
        if (Comparator.IsValidUpdate(FinishesAt, form.FinishesAt))
        {
            FinishesAt = (DateTime)form.FinishesAt;
            updated = true;
        }

        if (updated)
        {
            UpdatedAt = DateTime.UtcNow;
            return this;
        }

        return null;
    }
}

public class EventLocation
{
    public Guid Id { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    [Required]
    public string Country { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string PostalCode { get; set; }

    [Required]
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public EventLocation() { }

    public EventLocation Update(EventLocation updatedLocation)
    {
        Latitude = updatedLocation.Latitude;
        Longitude = updatedLocation.Longitude;
        Country = updatedLocation.Country;
        State = updatedLocation.State;
        City = updatedLocation.City;
        PostalCode = updatedLocation.PostalCode;
        AddressLine1 = updatedLocation.AddressLine1;
        AddressLine2 = updatedLocation.AddressLine2;
        UpdatedAt = DateTime.UtcNow;

        return this;
    }
}

public class EventTicketAllotment
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public Event Event { get; set; }

    [Required]
    public int Amount { get; set; } //PaymentAmount

    [Required]
    public int TicketsQuantityLimit { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public EventTicketAllotment() { }

    public EventTicketAllotment(CreateEventTicketAllotment form, Event @event)
    {
        Name = form.Name;
        Amount = form.PaymentAmount;
        TicketsQuantityLimit = form.TicketsQuantityLimit;
        Event = @event;
    }

    public EventTicketAllotment? Update(UpdateEventTicketAllotment form)
    {
        bool updated = false;
        if (Comparator.IsValidUpdate(Name, form.Name))
        {
            Name = form.Name;
            updated = true;
        }
        if (Comparator.IsValidUpdate(Amount, form.PaymentAmount))
        {
            Amount = form.PaymentAmount;
            updated = true;
        }
        if (Comparator.IsValidUpdate(TicketsQuantityLimit, form.TicketsQuantityLimit))
        {
            TicketsQuantityLimit = form.TicketsQuantityLimit;
            updated = true;
        }

        if (updated)
        {
            UpdatedAt = DateTime.UtcNow;
            return this;
        }

        return null;
    }
}
