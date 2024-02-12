using BungalowApi.Domain.Entities;
using BungalowApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BungalowApi.Web.Controllers;

public class BungalowController : Controller
{
    private readonly ApplicationDbContext _context;

    public BungalowController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var bungalows = _context.Bungalows.ToList();
        return View(bungalows);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(Bungalow bungalow)
    {
        if (ModelState.IsValid)
        {
            _context.Bungalows.Add(bungalow);
            _context.SaveChanges();
            TempData["success"] = "The Bungalow has been created successfully";
            return RedirectToAction("Index", "Bungalow");
        }
        return View();
    }
    public IActionResult Update(int bungalowId)
    {
        Bungalow bungalow = _context.Bungalows.FirstOrDefault(x => x.Id == bungalowId);
        if (bungalow is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(bungalow);
    }
    [HttpPost]
    public IActionResult Update(Bungalow bungalow)
    {
        if (ModelState.IsValid && bungalow.Id > 0)
        {
            _context.Bungalows.Update(bungalow);
            _context.SaveChanges();
            TempData["success"] = "The Bungalow has been updated successfully";
            return RedirectToAction("Index", "Bungalow");
        }
        return View();
    }
    public IActionResult Delete(int bungalowId)
    {
        Bungalow bungalow = _context.Bungalows.FirstOrDefault(x => x.Id == bungalowId);
        if (bungalow is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(bungalow);
    }
    [HttpPost]
    public IActionResult Delete(Bungalow bungalow)
    {
        Bungalow? deleteBungalow = _context.Bungalows.FirstOrDefault(u => u.Id == bungalow.Id);
        if (deleteBungalow is not null)
        {
            _context.Bungalows.Remove(deleteBungalow);
            _context.SaveChanges();
            TempData["success"] = "The Bungalow has been deleted successfully";
            return RedirectToAction("Index", "Bungalow");
        }
        TempData["error"] = "The Bungalow could not be deleted!";

        return View();
    }
}
