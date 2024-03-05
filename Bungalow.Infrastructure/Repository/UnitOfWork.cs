using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Infrastructure.Data;

namespace BungalowApi.Infrastructure.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IBungalowRepository Bungalow { get; private set; }
    public IBungalowNumberRepository BungalowNumber { get; private set; }
    public IAmenityRepository Amenity { get; private set; }
    public IBookingRepository Booking { get; private set; }
    public IApplicationUserRepository User { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Bungalow = new BungalowRepository(_context);
        BungalowNumber = new BungalowNumberRepository(_context);
        Amenity = new AmenityRepository(_context);
        Booking = new BookingRepository(_context);
        Booking = new BookingRepository(_context);
        User = new ApplicationUserRepository(_context);
    }
    public void Save()
    {
        _context.SaveChanges();
    }
}
