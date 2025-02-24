using BungalowApi.Application.Services.Interface;
using BungalowApi.Domain.Entities;
using Stripe.Checkout;

namespace BungalowApi.Application.Services.Implementation;

public class PaymentService : IPaymentService
{
    public Session CreateStripeSession(SessionCreateOptions options)
    {
        var service = new SessionService();
        Session session = service.Create(options);
        return session;
    }

    public SessionCreateOptions CreateStripeSessionOptions(Booking booking, Bungalow bungalow, string domain)
    {
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
            CancelUrl = domain +
                        $"booking/FinalizeBooking?bungalowId={booking.BungalowId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
        };


        options.LineItems.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (long)(booking.TotalCost * 100),
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = bungalow.Name
                    //Images = new List<string> { domain + bungalow.ImageUrl },
                },
            },
            Quantity = 1,
        });

        return options;
    }
}