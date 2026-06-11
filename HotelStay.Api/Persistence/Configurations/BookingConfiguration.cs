using HotelStay.Api.DomainEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelStay.Api.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reference)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(x => x.Reference)
            .IsUnique();

        builder.Property(x => x.Provider)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PassengerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Destination)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.RoomType)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.DocumentType)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.CancellationPolicy)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.TotalPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}