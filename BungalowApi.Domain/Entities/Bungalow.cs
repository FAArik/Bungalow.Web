﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BungalowApi.Domain.Entities;

public class Bungalow
{
    public int Id { get; set; }
    [MaxLength(50)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    [Display(Name = "Price Per Night")]
    [Range(10,10000)]
    public double Price { get; set; }
    public int Sqft { get; set; }
    [Range(1,10)]
    public int Occupancy { get; set; }
    [Display(Name ="Image Url")]
    public string ImageUrl { get; set; } 
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
