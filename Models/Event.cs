using OutsideServer.Forms;
using OutsideServer.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

    [Required]
    public EventLocation Location { get; set; }

    public List<EventTicketAllotment>? TicketAllotments { get; set; }

    [Required]
    public UserProfile CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Event() { }

    public Event(CreateEvent form, EventLocation location, UserProfile owner)
    {
        Name = form.Name;
        Description = form.Description;
        StartsAt = form.StartsAt;
        FinishesAt = form.FinishesAt;
        Location = location;
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


    public EventLocation(CreateEvent form)
    {
        Latitude = form.Latitude;
        Longitude = form.Longitude;
        Country = form.Country;
        State = form.State;
        City = form.City;
        PostalCode = form.PostalCode;
        AddressLine1 = form.AddressLine1;
        AddressLine2 = form.AddressLine2;
    }

    public EventLocation? Update(UpdateEvent form)
    {
        bool updated = false;

        if (Comparator.IsValidUpdate(Latitude, form.Latitude))
        {
            Latitude = (double)form.Latitude;
            updated = true;
        }
        if (Comparator.IsValidUpdate(Longitude, form.Longitude))
        {
            Longitude = (double)form.Longitude;
            updated = true;
        }
        if (Comparator.IsValidUpdate(Country, form.Country))
        {
            Country = form.Country;
            updated = true;
        }
        if (Comparator.IsValidUpdate(State, form.State))
        {
            State = form.State;
            updated = true;
        }
        if (Comparator.IsValidUpdate(City, form.City))
        {
            City = form.City;
            updated = true;
        }
        if (Comparator.IsValidUpdate(PostalCode, form.PostalCode))
        {
            PostalCode = form.PostalCode;
            updated = true;
        }
        if (Comparator.IsValidUpdate(AddressLine1, form.AddressLine1))
        {
            AddressLine1 = form.AddressLine1;
            updated = true;
        }
        if (Comparator.IsValidUpdate(AddressLine1, form.AddressLine2))
        {
            AddressLine2 = form.AddressLine2;
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
