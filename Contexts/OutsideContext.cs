using Microsoft.EntityFrameworkCore;
using OutsideServer.Models;

namespace OutsideServer.Contexts;

public class OutsideContext : DbContext
{
    public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventLocation> EventsLocations => Set<EventLocation>();
    public DbSet<EventTicketAllotment> EventTicketAllotments => Set<EventTicketAllotment>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentPixData> PaymentPixData => Set<PaymentPixData>();
    public DbSet<PaymentCreditCardData> PaymentCreditCardData => Set<PaymentCreditCardData>();
    public DbSet<PaymentBoletoData> PaymentBoletoData => Set<PaymentBoletoData>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=db.db3");
    }
}
