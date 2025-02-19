﻿using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Common.Utility;

public static class SD
{
    public const string Role_Customer = "Customer";
    public const string Role_Admin = "Admin";

    public const string StatusPending = "Pending";
    public const string StatusApproved = "Approved";
    public const string StatusCheckedIn = "CheckedIn";
    public const string StatusCompleted = "Completed";
    public const string StatusCanceled = "Canceled";
    public const string StatusRefunded = "Refunded";

    public static int BungalowRoomsAvailable_Count(int bungalowid, List<BungalowNumber> bungalowNumberList, DateOnly checkInDate, int nights, List<Booking> bookings)
    {
        List<int> bookingInDate = new();
        int finalAvailableRoom = int.MaxValue;
        var roomsInBungalow = bungalowNumberList.Where(x => x.BungalowId == bungalowid).Count();

        for (int i = 0; i < nights; i++)
        {
            var bungalowsBooked = bookings.Where(x => x.CheckInDate <= checkInDate.AddDays(i) && x.CheckOutDate > checkInDate.AddDays(i) && x.BungalowId == bungalowid);

            foreach (var booking in bungalowsBooked)
            {
                if (!bookingInDate.Contains(booking.Id))
                {
                    bookingInDate.Add(booking.Id);
                }
            }

            var totalAvailableRooms = roomsInBungalow - bookingInDate.Count;

            if (totalAvailableRooms == 0)
            {
                return 0;
            }
            else
            {
                if (finalAvailableRoom > totalAvailableRooms)
                {
                    finalAvailableRoom = totalAvailableRooms;
                }
            }
        }
        return finalAvailableRoom;
    }
}
