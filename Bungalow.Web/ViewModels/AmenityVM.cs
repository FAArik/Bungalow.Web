﻿using BungalowApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BungalowApi.Web.ViewModels;

public class AmenityVM
{
    public Amenity? Amenity { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> BungalowList { get; set; }
}
