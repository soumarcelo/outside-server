using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutsideServer.Contexts;
using OutsideServer.DTOs;
using OutsideServer.Filters;
using OutsideServer.Forms;
using OutsideServer.HTTPClients;
using OutsideServer.Models;

namespace OutsideServer.Controllers;

[Route("api/events")]
[AuthorizationFilter]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly OutsideContext _context;
    private readonly GeocodingAPI _geocodingAPIClient;

    private bool EventExists(Guid id) => _context.Events.Any(e => e.Id == id);

    private async Task<ActionResult<Event>> ValidateEventOwner(Guid eventId)
    {
        Guid? userId = (Guid?)RouteData.Values["userId"];
        if (userId is null) return Unauthorized();

        Event? @event = await _context.Events.FindAsync(eventId);
        if (@event is null) return NotFound();

        await _context.Events.Entry(@event)
            .Reference(e => e.CreatedBy)
            .LoadAsync();

        await _context.Events.Entry(@event)
            .Reference(e => e.Location)
            .LoadAsync();

        if (!@event.CreatedBy.Id.Equals(userId)) return Unauthorized();

        return @event;
    }

    private async Task<ActionResult<EventLocation>> GetLocationFromAddress(string addressLine1, string? addressLine2, string postalCode, string country)
    {
        string address = $"{addressLine1}, {postalCode}, {country}";
        GeocodingResponse? response =
            await _geocodingAPIClient.GetAddressInfo(address);

        if (response is null) return Forbid();
        if (response.status != "OK") return Forbid(); // Detalhar mais
        if (response.results.Count == 0) return Forbid(); // Detalhar mais
        if (response.results.Count > 1) return Forbid(); // Detalhar mais

        var addressData = response.results[0];

        string[] _addressLine1 = new string[3]; // route + street_number, + (locality || sublocality)
        string city = string.Empty; // administrative_area_level_2
        string state = string.Empty; // administrative_area_level_1
        string _country = string.Empty; // country

        foreach (var addressComponent in addressData.address_components)
        {
            if (addressComponent.types.Contains("route")) _addressLine1[0] = addressComponent.long_name;
            if (addressComponent.types.Contains("street_number")) _addressLine1[1] = addressComponent.long_name;
            if (
                addressComponent.types.Contains("locality")
                || addressComponent.types.Contains("sublocality")
                ) _addressLine1[2] = addressComponent.long_name;
            if (addressComponent.types.Contains("administrative_area_level_2")) city = addressComponent.long_name;
            if (addressComponent.types.Contains("administrative_area_level_1")) state = addressComponent.long_name;
            if (addressComponent.types.Contains("country")) _country = addressComponent.long_name;
        }

        return new EventLocation
        {
            Latitude = addressData.geometry.location.lat,
            Longitude = addressData.geometry.location.lng,
            Country = _country,
            State = state,
            City = city,
            PostalCode = postalCode,
            AddressLine1 = $"{_addressLine1[0]} {_addressLine1[1]}, {_addressLine1[2]}",
            AddressLine2 = addressLine2
        };
    }

    public EventsController(OutsideContext context, GeocodingAPI geocodingAPIClient)
    {
        _context = context;
        _geocodingAPIClient = geocodingAPIClient;
    }

    // GET: api/events
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<EventData>>> GetEvents()
    {
        List<Event> events = await _context.Events
        .Include(ev => ev.CreatedBy)
        .ThenInclude(up => up.User)
        .Include(ev => ev.Location)
        .ToListAsync();

        List<EventData> result = [];
        foreach (EventData @event in events) result.Add(@event);

        return Ok(result);
    }

    // GET: api/events/5
    [HttpGet("{eventId}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventData>> GetEvent(Guid eventId)
    {
        Event? @event = await _context.Events.FindAsync(eventId);
        if (@event is null) return NotFound();
        await _context.Events.Entry(@event)
            .Collection(e => e.TicketAllotments)
            .LoadAsync();

        await _context.Events.Entry(@event)
            .Reference(e => e.Location)
            .LoadAsync();

        await _context.Events.Entry(@event)
            .Reference(e => e.CreatedBy)
            .LoadAsync();

        await _context.UserProfiles.Entry(@event.CreatedBy)
            .Reference(up => up.User)
            .LoadAsync();

        return Ok((EventData)@event);
    }

    // PUT: api/events/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{eventId}")]
    public async Task<IActionResult> PutEvent(Guid eventId, UpdateEvent form)
    {
        ActionResult<Event> validationResult = await ValidateEventOwner(eventId);
        if (validationResult.Value is null) return validationResult.Result;
        
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            Event @event = validationResult.Value;

            

            Event? updatedEvent = @event.Update(form);
            if (updatedEvent is not null)
            {
                _context.Entry(updatedEvent).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            transaction.Commit();

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            transaction.Rollback();
            throw;
        }

    }

    // POST: api/events
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<EventData>> PostEvent(CreateEvent form)
    {
        Guid? userId = (Guid?)RouteData.Values["userId"];
        if (userId is null) return Unauthorized();

        UserProfile? userProfile = await _context.UserProfiles.FindAsync(userId);
        if (userProfile is null) return Unauthorized();
        await _context.UserProfiles.Entry(userProfile)
            .Reference(up => up.User)
            .LoadAsync();

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            Event @event = new(form, userProfile);

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();
            transaction.Commit();

            return CreatedAtAction(nameof(GetEvent), new { eventId = @event.Id }, (EventData)@event);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // DELETE: api/events/5
    [HttpDelete("{eventId}")]
    public async Task<IActionResult> DeleteEvent(Guid eventId)
    {
        ActionResult<Event> validationResult = await ValidateEventOwner(eventId);
        if (validationResult.Value is null) return validationResult.Result;

        Event @event = validationResult.Value;

        _context.Events.Remove(@event);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/events/{id}/location
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{eventId}/location")]
    public async Task<ActionResult<EventTicketAllotmentData>> PutEventLocation(Guid eventId, UpdateEventLocation form)
    {
        ActionResult<Event> validationResult = await ValidateEventOwner(eventId);
        if (validationResult.Value is null) return validationResult.Result;

        EventLocation? location = validationResult.Value.Location;
        if (location is null) return NotFound();

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            bool needUpdate = false;
            if (!location.Country.Equals(form.Country)) needUpdate = true;
            if (!location.PostalCode.Equals(form.PostalCode)) needUpdate = true;
            if (!location.AddressLine1.Equals(form.AddressLine1)) needUpdate = true;

            if (!needUpdate) return NoContent();

            ActionResult<EventLocation> locationResult =
                await GetLocationFromAddress(
                    form.AddressLine1 ?? location.AddressLine1, 
                    form.AddressLine2, 
                    form.PostalCode ?? location.PostalCode, 
                    form.Country ?? location.Country);
            if (locationResult.Value is null) return locationResult.Result;

            EventLocation updatedLocation = location.Update(locationResult.Value);
        
            _context.Entry(updatedLocation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            transaction.Commit();

            return NoContent();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // POST: api/events/{id}/location
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("{eventId}/location")]
    public async Task<ActionResult<EventLocationData>> PostEventLocation(Guid eventId, CreateEventLocation form)
    {
        ActionResult<Event> validationResult = await ValidateEventOwner(eventId);
        if (validationResult.Value is null) return validationResult.Result;

        Event @event = validationResult.Value;
        if (@event.Location is not null) return Forbid();

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            ActionResult<EventLocation> locationResult = 
                await GetLocationFromAddress(form.AddressLine1, form.AddressLine2, form.PostalCode, form.Country);
            if (locationResult.Value is null) return locationResult.Result;

            EventLocation location = locationResult.Value;
            @event.Location = location;
            @event.UpdatedAt = DateTime.UtcNow;

            _context.EventsLocations.Add(location);
            _context.Entry(@event).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            transaction.Commit();

            return CreatedAtAction(nameof(GetEventLocation), new { eventId }, (EventLocationData)location);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    
    // GET: api/events/{id}/location
    [HttpGet("{eventId}/location")]
    public async Task<ActionResult<EventLocationData>> GetEventLocation(Guid eventId)
    {
        ActionResult<Event> validationResult = await ValidateEventOwner(eventId);
        if (validationResult.Value is null) return validationResult.Result;

        EventLocation? location = validationResult.Value.Location;
        if (location is null) return NotFound();

        return Ok((EventLocationData)location);
    }

    [AllowAnonymous]
    [HttpGet("{eventId}/ticket_allotments")]
    public async Task<ActionResult<List<EventTicketAllotmentData>>> GetEventTicketAllotments(Guid eventId)
    {
        if (!EventExists(eventId)) return NotFound();

        List<EventTicketAllotment> ticketAllotments = await _context.EventTicketAllotments
            .Where((eta) => eta.Event.Id == eventId)
            .ToListAsync();

        List<EventTicketAllotmentData> result = [];
        foreach (EventTicketAllotmentData ticketAllotment in ticketAllotments)
            result.Add(ticketAllotment);

        return Ok(result);
    }

    // GET: api/events/5/ticket_allotments/1
    // Is it necessary?
    [HttpGet("{eventId}/ticket_allotments/{allotmentId}")]
    public async Task<ActionResult<EventTicketAllotmentData>> GetEventTicketAllotment(Guid eventId, Guid allotmentId)
    {
        if (!EventExists(eventId)) return NotFound();

        EventTicketAllotment? ticketAllotment = await _context.EventTicketAllotments.FindAsync(allotmentId);
        if (ticketAllotment is null) return NotFound();

        return Ok(ticketAllotment);
    }

    [HttpPut("{eventId}/ticket_allotments/{allotmentId}")]
    public async Task<IActionResult> PutEventTicketAllotment(Guid eventId, Guid allotmentId, UpdateEventTicketAllotment form) 
    {
        if (!EventExists(eventId)) return NotFound();

        EventTicketAllotment? ticketAllotment = await _context.EventTicketAllotments.FindAsync(allotmentId);
        if (ticketAllotment is null) return NotFound();

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            EventTicketAllotment? updatedTicketallotment = ticketAllotment.Update(form);
            if (updatedTicketallotment is not null)
            {
                _context.Entry(updatedTicketallotment).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            transaction.Commit();
            return NoContent();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // POST: api/events/5/ticket_allotments
    [HttpPost("{eventId}/ticket_allotments")]
    public async Task<ActionResult<EventTicketAllotmentData>> PostEventTicketAllotment(Guid eventId, CreateEventTicketAllotment form)
    {
        ActionResult<Event> validationResult = await ValidateEventOwner(eventId);
        if (validationResult.Value is null) return validationResult.Result;
        
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            Event @event = validationResult.Value;
            EventTicketAllotment ticketAllotment = new(form, @event);

            _context.EventTicketAllotments.Add(ticketAllotment);
            await _context.SaveChangesAsync();

            transaction.Commit();
            return CreatedAtAction(nameof(GetEventTicketAllotment), new { eventId, allotmentId = ticketAllotment.Id }, (EventTicketAllotmentData)ticketAllotment);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    [HttpDelete("{eventId}/ticket_allotments/{allotmentId}")]
    public async Task<IActionResult> DeleteEventTicketAllotment(Guid eventId, Guid allotmentId) 
    {
        if (!EventExists(eventId)) return NotFound();

        EventTicketAllotment? ticketAllotment = await _context.EventTicketAllotments.FindAsync(allotmentId);
        if (ticketAllotment is null) return NotFound();

        _context.EventTicketAllotments.Remove(ticketAllotment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
