using HotelStay.Api.DomainEntities;
using HotelStay.Api.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HotelStay.Api.Persistence;

public sealed class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options)
        : base(options)
    {
    }

    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BookingConfiguration());
    }
}
