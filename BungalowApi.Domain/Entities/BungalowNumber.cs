using System.ComponentModel.DataAnnotations;

namespace BungalowApi.Domain.Entities;

public class BungalowNumber
{
    [Key]
    public int Bungalow_Number { get; set; }
    public Bungalow Bungalow { get; set; }

}
