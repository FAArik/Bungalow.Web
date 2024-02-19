using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BungalowApi.Domain.Entities;

public class Amenity
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    [ForeignKey("Bungalow")]
    public int BungalowId { get; set; }
    [ValidateNever]
    public Bungalow Bungalow { get; set; }
}
