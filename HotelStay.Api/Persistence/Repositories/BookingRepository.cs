using HotelStay.Api.DomainEntities;
using Microsoft.EntityFrameworkCore;

namespace HotelStay.Api.Persistence.Repositories;

public sealed class BookingRepository : IBookingRepository
{
    private readonly HotelDbContext dbContext;

    public BookingRepository(HotelDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        return dbContext.Bookings.AddAsync(booking, cancellationToken).AsTask();
    }

    public Task<Booking?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        return dbContext.Bookings.AsNoTracking().FirstOrDefaultAsync(booking => booking.Reference == reference, cancellationToken);
    }

    public Task<bool> ReferenceExistsAsync(string reference, CancellationToken cancellationToken = default)
    {
        return dbContext.Bookings.AnyAsync(booking => booking.Reference == reference, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}