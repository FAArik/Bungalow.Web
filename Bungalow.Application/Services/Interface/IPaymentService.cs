using BungalowApi.Domain.Entities;
using Stripe.Checkout;

namespace BungalowApi.Application.Services.Interface;

public interface IPaymentService
{
    SessionCreateOptions CreateStripeSessionOptions(Booking booking, Bungalow bungalow, string domain);
    Session CreateStripeSession(SessionCreateOptions options);
}