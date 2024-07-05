using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutsideServer.Contexts;
using OutsideServer.DTOs;
using OutsideServer.Filters;
using OutsideServer.Forms;
using OutsideServer.Models;
using System.Collections.Generic;
using System.Linq;

namespace OutsideServer.Controllers;

[Route("api/events")]
[AuthorizationFilter]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly OutsideContext _context;

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

    public EventsController(OutsideContext context)
    {
        _context = context;
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

            EventLocation? updatedEventLocation = @event.Location.Update(form);
            if (updatedEventLocation is not null)
            {
                _context.Entry(updatedEventLocation).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

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

            if (!EventExists(eventId)) return NotFound();
            else throw;
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
            EventLocation location = new(form);
            Event @event = new(form, location, userProfile);

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
}
