using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutsideServer.Contexts;
using OutsideServer.DTOs;
using OutsideServer.Filters;
using OutsideServer.Forms;
using OutsideServer.Models;

namespace OutsideServer.Controllers;

[Route("api/users")]
[AuthorizationFilter]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly OutsideContext _context;

    private bool UserProfileExists(Guid id)
        => _context.UserProfiles.Any(e => e.Id == id);

    private async Task<UserData?> GetUserById(Guid id)
    {
        UserProfile? profile = await _context.UserProfiles.FindAsync(id);
        if (profile is null) return null;
        await _context.UserProfiles.Entry(profile)
            .Reference(up => up.User)
            .LoadAsync();

        UserData userData = (UserData)profile;

        return userData;
    }

    public UsersController(OutsideContext context)
    {
        _context = context;
    }

    // GET: api/users
    // ONLY ADMIN ROLE
    [HttpGet]
    public async Task<ActionResult<List<UserData>>> GetUsers()
        => await _context.UserProfiles.Select((user, index) => (UserData)user).ToListAsync();

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserData>> GetUser(Guid id)
    {
        UserData? userData = await GetUserById(id);
        if (userData is null) return NotFound();
        return userData;
    }

    // GET: api/users/current
    [HttpGet("current")]
    public async Task<ActionResult<UserData>> GetCurrentUser()
    {
        Guid id = (Guid)RouteData.Values["userId"];
        
        UserData? userData = await GetUserById(id);
        if (userData is null) return NotFound();
        return userData;
    }

    // PUT: api/users/current
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("current")]
    public async Task<IActionResult> PutCurrentUser(UpdateUser form)
    {
        using var transaction = _context.Database.BeginTransaction();
        
        Guid id = (Guid)RouteData.Values["userId"];

        try
        {
            UserProfile? profile = await _context.UserProfiles.FindAsync(id);
            if (profile is null) return NotFound();
            await _context.UserProfiles.Entry(profile)
                .Reference(up => up.User)
                .LoadAsync();

            UserIdentity? updatedUser = profile.User.Update(form);
            if (updatedUser is not null)
            {
                _context.Entry(updatedUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            UserProfile? updatedProfile = profile.Update(form);
            if (updatedProfile is not null)
            {
                _context.Entry(updatedProfile).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            transaction.Commit();

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            transaction.Rollback();

            if (!UserProfileExists(id)) return NotFound();
            else throw;
        }
    }

    // DELETE: api/users/current
    [HttpDelete("current")]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        Guid id = (Guid)RouteData.Values["userId"];

        UserProfile? profile = await _context.UserProfiles.FindAsync(id);
        if (profile is null) return NotFound();

        _context.UserProfiles.Remove(profile);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
