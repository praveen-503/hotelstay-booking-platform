using HotelStay.Api.DomainEntities;

namespace HotelStay.Api.Persistence.Repositories;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);

    Task<Booking?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);

    Task<bool> ReferenceExistsAsync(string reference, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}