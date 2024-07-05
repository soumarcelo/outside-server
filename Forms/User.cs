using System.ComponentModel.DataAnnotations;

namespace OutsideServer.Forms;

public class SignInRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class SignInResponse
{
    public required string Token { get; set; }
}

public class CreateUser : SignInRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

public class UpdateUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}
