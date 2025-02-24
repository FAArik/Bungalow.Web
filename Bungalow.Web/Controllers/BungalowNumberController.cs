using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Services.Interface;
using BungalowApi.Domain.Entities;
using BungalowApi.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BungalowApi.Web.Controllers;

public class BungalowNumberController : Controller
{
    private readonly IBungalowNumberService _bungalowNumberService;
    private readonly IBungalowService _bungalowService;

    public BungalowNumberController(IBungalowNumberService bungalowNumberService, IBungalowService bungalowService)
    {
        _bungalowNumberService = bungalowNumberService;
        _bungalowService = bungalowService;
    }

    public IActionResult Index()
    {
        var bungalowNumbers = _bungalowNumberService.GetAllBungalowNumbers().ToList();
        return View(bungalowNumbers);
    }

    public IActionResult Create()
    {
        BungalowNumberVM bungalowNumberVM = new()
        {
            BungalowList = _bungalowService.GetAllBungalow().Select(x => new SelectListItem
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
        bool roomNumberExists =
            _bungalowNumberService.RoomNumberExists(bungalowNumbervm.BungalowNumber.Bungalow_Number);

        if (ModelState.IsValid && !roomNumberExists)
        {
            _bungalowNumberService.CreateBungalowNumber(bungalowNumbervm.BungalowNumber);
            TempData["success"] = "The Bungalow number has been created successfully";
            return RedirectToAction(nameof(Index));
        }

        if (roomNumberExists)
        {
            TempData["error"] = "The Bungalow number already exists ";
        }

        bungalowNumbervm.BungalowList = _bungalowService.GetAllBungalow().Select(x => new SelectListItem
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
            BungalowNumber = _bungalowNumberService.GetBungalowNumberById(bungalowNumberId),
            BungalowList = _bungalowService.GetAllBungalow().Select(x => new SelectListItem
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
            _bungalowNumberService.UpdateBungalowNumber(bungalowNumbervm.BungalowNumber);
            TempData["success"] = "The BungalowNumber has been updated successfully";
            return RedirectToAction(nameof(Index));
        }

        bungalowNumbervm.BungalowList = _bungalowService.GetAllBungalow().Select(x => new SelectListItem
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
            BungalowNumber = _bungalowNumberService.GetBungalowNumberById(bungalowNumberId),
            BungalowList = _bungalowService.GetAllBungalow().Select(x => new SelectListItem
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
        BungalowNumber? deletebungalowNumber =
            _bungalowNumberService.GetBungalowNumberById(bungalowNumbervm.BungalowNumber.Bungalow_Number);
        if (deletebungalowNumber is not null)
        {
            _bungalowNumberService.DeleteBungalowNumber(deletebungalowNumber.Bungalow_Number);
            TempData["success"] = "The BungalowNumber has been deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The BungalowNumber could not be deleted!";
        return View();
    }
}