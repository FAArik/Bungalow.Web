using BungalowApi.Domain.Entities;
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
    public IActionResult Create(BungalowNumberVM bungalowNumbervm)
    {
        bool roomNumberExists = _context.BungalowNumbers.Any(x => x.Bungalow_Number == bungalowNumbervm.BungalowNumber.Bungalow_Number);

        if (ModelState.IsValid && !roomNumberExists)
        {
            _context.BungalowNumbers.Add(bungalowNumbervm.BungalowNumber);
            _context.SaveChanges();
            TempData["success"] = "The Bungalow number has been created successfully";
            return RedirectToAction(nameof(Index));
        }
        if (roomNumberExists)
        {
            TempData["error"] = "The Bungalow number already exists ";
        }
        bungalowNumbervm.BungalowList = _context.Bungalows.ToList().Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        });
        return View(bungalowNumbervm);
    }
    public IActionResult Update(int bungalowNumberId)
    {
        BungalowNumberVM bungalowNumbervm = new()
        {
            BungalowNumber = _context.BungalowNumbers.FirstOrDefault(x => x.Bungalow_Number == bungalowNumberId),
            BungalowList = _context.Bungalows.ToList().Select(x => new SelectListItem
             {
                 Text = x.Name,
                 Value = x.Id.ToString()
             })
        };

        if (bungalowNumbervm.BungalowNumber is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(bungalowNumbervm);
    }
    [HttpPost]
    public IActionResult Update(BungalowNumberVM bungalowNumbervm)
    {

        if (ModelState.IsValid)
        {
            _context.BungalowNumbers.Update(bungalowNumbervm.BungalowNumber);
            _context.SaveChanges();
            TempData["success"] = "The BungalowNumber has been updated successfully";
            return RedirectToAction(nameof(Index));
        }
        bungalowNumbervm.BungalowList = _context.Bungalows.ToList().Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        });
        return View(bungalowNumbervm);
    }
    public IActionResult Delete(int bungalowNumberId)
    {

        BungalowNumberVM bungalowNumbervm = new()
        {
            BungalowNumber = _context.BungalowNumbers.FirstOrDefault(x => x.Bungalow_Number == bungalowNumberId),
        BungalowList = _context.Bungalows.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
        };
        if (bungalowNumbervm.BungalowNumber is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(bungalowNumbervm);
    }
    [HttpPost]
    public IActionResult Delete(BungalowNumberVM bungalowNumbervm)
    {
        BungalowNumber? deletebungalowNumber = _context.BungalowNumbers.FirstOrDefault(u => u.Bungalow_Number == bungalowNumbervm.BungalowNumber.Bungalow_Number);
        if (deletebungalowNumber is not null)
        {
            _context.BungalowNumbers.Remove(deletebungalowNumber);
            _context.SaveChanges();
            TempData["success"] = "The BungalowNumber has been deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "The BungalowNumber could not be deleted!";

        return View();
    }
}
