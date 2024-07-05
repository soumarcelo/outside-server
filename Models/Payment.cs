using OutsideServer.Forms;
using System.ComponentModel.DataAnnotations;

namespace OutsideServer.Models;

public class Payment
{
    public enum PaymentMethod { Pix, CreditCard, Boleto }
    public enum PaymentStatus { None, Created, Pending, Processing, Finished, Expired, Cancelled, Error }

    public Guid Id { get; set; }
    [Required]
    public int Amount { get; set; }

    [Required]
    public PaymentMethod Method { get; set; }

    [Required]
    public PaymentStatus Status { get; set; } = PaymentStatus.None;
    public PaymentPixData? PixData {  get; set; }
    public PaymentCreditCardData? CreditCardData {  get; set; }
    public PaymentBoletoData? BoletoData {  get; set; }

    [Required]
    public UserProfile PaidBy { get; set; }
    public List<Ticket>? Tickets { get; set; }
    public string? ErrorDetail { get; set; }

    [Required]
    public DateTime ExpiresAt { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Payment() { }

    public Payment(CreatePayment form, UserProfile payer)
    {
        Amount = form.PaymentAmount;
        Method = form.Method;
        Status = PaymentStatus.Created;
        PaidBy = payer;
        ExpiresAt = DateTime.UtcNow.AddMinutes(15);
    }
}

public class PaymentPixData
{
    public Guid Id { get; set; }

    [Required]
    public string QrCode { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PaymentCreditCardData
{
    public Guid Id { get; set; }

    [Required]
    public string Number { get; set; }

    [Required]
    public string CVC { get; set; }

    [Required]
    public string OwnerName { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PaymentBoletoData
{
    public Guid Id { get; set; }

    [Required]
    public string Barcode { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
