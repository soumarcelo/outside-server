using System.ComponentModel.DataAnnotations;

namespace OutsideServer.Models;

public class Ticket
{
    public Guid Id { get; set; }
    [Required]
    public UserProfile Owner { get; set; }

    [Required]
    public EventTicketAllotment Allotment { get; set; }

    [Required]
    public Payment Payment { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Ticket() { }

    public Ticket(UserProfile owner, EventTicketAllotment allotment, Payment payment)
    {
        Owner = owner;
        Allotment = allotment;
        Payment = payment;
    }
}
