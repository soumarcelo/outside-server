using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OutsideServer.Contexts;
using OutsideServer.DTOs;
using OutsideServer.Forms;
using OutsideServer.Models;

namespace OutsideServer.Controllers;

[Route("api/signup")]
[ApiController]
public class SignUpController : ControllerBase
{
    private readonly OutsideContext _context;

    private bool UserIdentityExists(string email)
        => _context.UserIdentities.Any(e => e.Email == email);

    public SignUpController(OutsideContext context)
    {
        _context = context;
    }

    // POST: api/signup
    [HttpPost]
    public async Task<ActionResult<UserData>> PostSignUp(CreateUser data)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            if (UserIdentityExists(data.Email))
                return BadRequest();

            UserIdentity user = new(data);
            PasswordHasher<UserIdentity> passwordHasher = new PasswordHasher<UserIdentity>();
            string hashedPassword = passwordHasher.HashPassword(user, user.Password);
            user.Password = hashedPassword;

            UserProfile profile = new(data, user);

            _context.UserIdentities.Add(user);
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            UserData response = (UserData)profile;

            transaction.Commit();

            return response;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
