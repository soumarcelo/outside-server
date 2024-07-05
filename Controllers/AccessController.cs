using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OutsideServer.Contexts;
using OutsideServer.Filters;
using OutsideServer.Forms;
using OutsideServer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OutsideServer.Controllers;

[Route("api")]
[ApiController]
public class AccessController : ControllerBase
{
    private readonly OutsideContext _context;
    private readonly IConfiguration _configuration;

    public AccessController(OutsideContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration; 
    }

    // POST: api/signin
    [HttpPost("signin")]
    public async Task<ActionResult<SignInResponse>> PostSignIn(SignInRequest form) 
    {
        UserProfile? profile = _context.UserProfiles.SingleOrDefault(up => up.User.Email == form.Email);
        if (profile is null) return NotFound();
        await _context.UserProfiles.Entry(profile)
            .Reference(up => up.User)
            .LoadAsync();

        PasswordHasher<UserIdentity> passwordHasher = new PasswordHasher<UserIdentity>();
        PasswordVerificationResult result = 
            passwordHasher.VerifyHashedPassword(profile.User, profile.User.Password, form.Password);

        if (result == PasswordVerificationResult.Failed) return Unauthorized();

        string issuer = _configuration["Jwt:Issuer"];
        string audience = _configuration["Jwt:Audience"];
        byte[] _key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id",Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, profile.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, form.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            }),

            Expires = DateTime.UtcNow.AddHours(8),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = 
                new SigningCredentials(
                    new SymmetricSecurityKey(_key),
                    SecurityAlgorithms.HmacSha256Signature
                )
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string jwtToken = tokenHandler.WriteToken(token);

        return new SignInResponse() { Token = jwtToken };
    }

    [HttpPost("logout")]
    [AuthorizationFilter]
    public ActionResult PostLogOut()
    {
        return NoContent();
    }
}
