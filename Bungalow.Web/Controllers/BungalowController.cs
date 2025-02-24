using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Services.Interface;
using BungalowApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BungalowApi.Web.Controllers;

public class BungalowController : Controller
{
    private readonly IBungalowService _bungalowService;

    public BungalowController(IBungalowService bungalowService)
    {
        _bungalowService = bungalowService;
    }

    public IActionResult Index()
    {
        var bungalows = _bungalowService.GetAllBungalow();
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
            _bungalowService.CreateBungalow(bungalow);
            TempData["success"] = "The Bungalow has been created successfully";
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult Update(int bungalowId)
    {
        Bungalow bungalow = _bungalowService.GetBungalowById(bungalowId);
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
            _bungalowService.UpdateBungalow(bungalow);
            TempData["success"] = "The Bungalow has been updated successfully";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    public IActionResult Delete(int bungalowId)
    {
        Bungalow bungalow = _bungalowService.GetBungalowById(bungalowId);
        if (bungalow is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View();
    }

    [HttpPost]
    public IActionResult Delete(Bungalow bungalow)
    {
        bool isDeleted = _bungalowService.DeleteBungalow(bungalow.Id);
        if (isDeleted)
        {
            TempData["success"] = "The Bungalow has been deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The Bungalow could not be deleted!";
        return View();
    }
}