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

    public DbSet<Hotel> Hotels => Set<Hotel>();

    public DbSet<Room> Rooms => Set<Room>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BookingConfiguration());

        modelBuilder.Entity<Hotel>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Country)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.AddressLine1)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.AddressLine2)
                .HasMaxLength(250)
                .IsRequired(false);

            builder.HasMany(x => x.Rooms)
                .WithOne(x => x.Hotel)
                .HasForeignKey(x => x.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Room>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RoomNumber)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(x => x.RoomType)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Ignore(x => x.Bookings);
        });
    }
}
