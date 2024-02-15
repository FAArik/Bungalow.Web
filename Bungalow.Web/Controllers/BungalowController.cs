using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Domain.Entities;
using BungalowApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BungalowApi.Web.Controllers;

public class BungalowController : Controller
{
    private readonly IBungalowRepository _bungalowRepository;

    public BungalowController(IBungalowRepository bungalowRepository)
    {
        _bungalowRepository = bungalowRepository;
    }

    public IActionResult Index()
    {
        var bungalows = _bungalowRepository.GetAll();
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
            _bungalowRepository.Add(bungalow);
            _bungalowRepository.Save();
            TempData["success"] = "The Bungalow has been created successfully";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }
    public IActionResult Update(int bungalowId)
    {
        Bungalow bungalow = _bungalowRepository.Get(x => x.Id == bungalowId);
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
            _bungalowRepository.Update(bungalow);
            _bungalowRepository.Save();
            TempData["success"] = "The Bungalow has been updated successfully";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }
    public IActionResult Delete(int bungalowId)
    {
        Bungalow bungalow = _bungalowRepository.Get(x => x.Id == bungalowId);
        if (bungalow is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(bungalow);
    }
    [HttpPost]
    public IActionResult Delete(Bungalow bungalow)
    {
        Bungalow? deleteBungalow = _bungalowRepository.Get(u => u.Id == bungalow.Id);
        if (deleteBungalow is not null)
        {
            _bungalowRepository.Delete(deleteBungalow);
            _bungalowRepository.Save();
            TempData["success"] = "The Bungalow has been deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "The Bungalow could not be deleted!";

        return View();
    }
}
