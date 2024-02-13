using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BungalowApi.Domain.Entities;

public class BungalowNumber
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Bungalow Number")]
    public int Bungalow_Number { get; set; }
    [ForeignKey("Bungalow")]
    public int BungalowId { get; set; }
    [ValidateNever]
    public Bungalow Bungalow { get; set; }
    public string? SpecialDetails { get; set; }
}
