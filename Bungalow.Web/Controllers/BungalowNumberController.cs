﻿using BungalowApi.Domain.Entities;
using BungalowApi.Infrastructure.Data;
using BungalowApi.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BungalowApi.Web.Controllers;

public class BungalowNumberController : Controller
{
    private readonly ApplicationDbContext _context;

    public BungalowNumberController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var bungalowNumbers = _context.BungalowNumbers.Include(x => x.Bungalow).ToList();
        return View(bungalowNumbers);
    }
    public IActionResult Create()
    {
        BungalowNumberVM bungalowNumberVM = new()
        {
            BungalowList = _context.Bungalows.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }),
        };
        return View(bungalowNumberVM);
    }
    [HttpPost]
    public IActionResult Create(BungalowNumberVM bungalowNumber)
    {
        bool roomNumberExists = _context.BungalowNumbers.Any(x => x.Bungalow_Number == bungalowNumber.BungalowNumber.Bungalow_Number);

        if (ModelState.IsValid && !roomNumberExists)
        {
            _context.BungalowNumbers.Add(bungalowNumber.BungalowNumber);
            _context.SaveChanges();
            TempData["success"] = "The Bungalow number has been created successfully";
            return RedirectToAction("Index", "BungalowNumber");
        }
        if (roomNumberExists)
        {
            TempData["error"] = "The Bungalow number already exists ";
        }
        bungalowNumber.BungalowList = _context.Bungalows.ToList().Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        });
        return View(bungalowNumber);
    }
    public IActionResult Update(int bungalowNumber)
    {
        BungalowNumber bungalow = _context.BungalowNumbers.FirstOrDefault(x => x.Bungalow_Number == bungalowNumber);
        if (bungalow is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(bungalow);
    }
    [HttpPost]
    public IActionResult Update(BungalowNumber bungalowNumber)
    {
        if (ModelState.IsValid && bungalowNumber.Bungalow_Number > 0)
        {
            _context.BungalowNumbers.Update(bungalowNumber);
            _context.SaveChanges();
            TempData["success"] = "The BungalowNumber has been updated successfully";
            return RedirectToAction("Index", "Bungalow");
        }
        return View();
    }
    public IActionResult Delete(int bungalowNumber)
    {
        BungalowNumber bungalow_Number = _context.BungalowNumbers.FirstOrDefault(x => x.Bungalow_Number == bungalowNumber);
        if (bungalow_Number is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(bungalow_Number);
    }
    [HttpPost]
    public IActionResult Delete(BungalowNumber bungalowNumber)
    {
        BungalowNumber? deletebungalowNumber = _context.BungalowNumbers.FirstOrDefault(u => u.Bungalow_Number == bungalowNumber.Bungalow_Number);
        if (deletebungalowNumber is not null)
        {
            _context.BungalowNumbers.Remove(bungalowNumber);
            _context.SaveChanges();
            TempData["success"] = "The BungalowNumber has been deleted successfully";
            return RedirectToAction("Index", "Bungalow");
        }
        TempData["error"] = "The BungalowNumber could not be deleted!";

        return View();
    }
}
