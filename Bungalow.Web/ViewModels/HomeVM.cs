using BungalowApi.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace BungalowApi.Web.ViewModels;
public class HomeVM
{
    public IEnumerable<Bungalow>? BungalowList { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly? CheckOutDate { get; set; }
    public int Nights { get; set; }

}
