using static OutsideServer.Models.Payment;

namespace OutsideServer.Forms;

public class CreatePayment
{
    public required List<RequestedTicket> RequestedTickets { get; set; }
    public required int PaymentAmount { get; set; }
    public PaymentMethod Method { get; set; }
}

public class RequestedTicket
{
    public required string EventAllotmentId { get; set; }
    public required int Quantity { get; set; }
}
