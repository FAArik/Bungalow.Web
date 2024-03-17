using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Common.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    void Update(Booking booking);
    void UpdateStatus(int bookingId, string bookingStatus, int bungalowNumber);
    void UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId);
}
