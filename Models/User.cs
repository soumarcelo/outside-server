using OutsideServer.Forms;
using OutsideServer.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OutsideServer.Models;

public class UserIdentity
{
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [JsonIgnore]
    public string Password { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public UserIdentity() { }

    public UserIdentity(CreateUser form)
    {
        Email = form.Email;
        Password = form.Password;
    }

    public UserIdentity? Update(UpdateUser form)
    {
        if (string.IsNullOrEmpty(form.Email) || form.Email.Equals(Email)) return null;

        Email = form.Email;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }
}

public class UserProfile
{
    public Guid Id { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public UserIdentity User { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public UserProfile() { }

    public UserProfile(CreateUser form, UserIdentity user)
    {
        FirstName = form.FirstName;
        LastName = form.LastName;
        User = user;
    }

    public UserProfile? Update(UpdateUser form)
    {
        bool updated = false;
        if (Comparator.IsValidUpdate(FirstName, form.FirstName))
        {
            FirstName = form.FirstName;
            updated = true;
        }
        if (Comparator.IsValidUpdate(LastName, form.LastName))
        {
            LastName = form.LastName;
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
