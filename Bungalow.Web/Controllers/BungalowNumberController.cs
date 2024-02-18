using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Domain.Entities;
using BungalowApi.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BungalowApi.Web.Controllers;

public class BungalowNumberController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public BungalowNumberController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var bungalowNumbers = _unitOfWork.BungalowNumber.GetAll(includeProperties: "Bungalow").ToList();
        return View(bungalowNumbers);
    }
    public IActionResult Create()
    {
        BungalowNumberVM bungalowNumberVM = new()
        {
            BungalowList = _unitOfWork.Bungalow.GetAll().Select(x => new SelectListItem
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
        bool roomNumberExists = _unitOfWork.BungalowNumber.Any(x => x.Bungalow_Number == bungalowNumbervm.BungalowNumber.Bungalow_Number);

        if (ModelState.IsValid && !roomNumberExists)
        {
            _unitOfWork.BungalowNumber.Add(bungalowNumbervm.BungalowNumber);
            _unitOfWork.Save();
            TempData["success"] = "The Bungalow number has been created successfully";
            return RedirectToAction(nameof(Index));
        }
        if (roomNumberExists)
        {
            TempData["error"] = "The Bungalow number already exists ";
        }
        bungalowNumbervm.BungalowList = _unitOfWork.Bungalow.GetAll().Select(x => new SelectListItem
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
            BungalowNumber = _unitOfWork.BungalowNumber.Get(x => x.Bungalow_Number == bungalowNumberId),
            BungalowList = _unitOfWork.Bungalow.GetAll().Select(x => new SelectListItem
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
            _unitOfWork.BungalowNumber.Update(bungalowNumbervm.BungalowNumber);
            _unitOfWork.Save();
            TempData["success"] = "The BungalowNumber has been updated successfully";
            return RedirectToAction(nameof(Index));
        }
        bungalowNumbervm.BungalowList = _unitOfWork.Bungalow.GetAll().Select(x => new SelectListItem
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
            BungalowNumber = _unitOfWork.BungalowNumber.Get(x => x.Bungalow_Number == bungalowNumberId),
            BungalowList = _unitOfWork.Bungalow.GetAll().Select(x => new SelectListItem
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
        BungalowNumber? deletebungalowNumber = _unitOfWork.BungalowNumber.Get(u => u.Bungalow_Number == bungalowNumbervm.BungalowNumber.Bungalow_Number);
        if (deletebungalowNumber is not null)
        {
            _unitOfWork.BungalowNumber.Delete(deletebungalowNumber);
            _unitOfWork.Save();
            TempData["success"] = "The BungalowNumber has been deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "The BungalowNumber could not be deleted!";

        return View();
    }
}
