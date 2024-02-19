using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Domain.Entities;
using BungalowApi.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BungalowApi.Web.Controllers;

public class AmenityController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AmenityController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var amenities = _unitOfWork.Amenity.GetAll(includeProperties: "Bungalow").ToList();
        return View(amenities);
    }
    public IActionResult Create()
    {
        AmenityVM amenityVM = new()
        {
            BungalowList = _unitOfWork.Bungalow.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }),
        };
        return View(amenityVM);
    }
    [HttpPost]
    public IActionResult Create(AmenityVM amenityvm)
    {

        if (ModelState.IsValid)
        {
            _unitOfWork.Amenity.Add(amenityvm.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "The Amenity has been created successfully";
            return RedirectToAction(nameof(Index));
        }
       
        amenityvm.BungalowList = _unitOfWork.Bungalow.GetAll().Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        });
        return View(amenityvm);
    }
    public IActionResult Update(int amenityId)
    {
        AmenityVM amenityvm = new()
        {
            Amenity = _unitOfWork.Amenity.Get(x => x.Id == amenityId),
            BungalowList = _unitOfWork.Bungalow.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
        };

        if (amenityvm.Amenity is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(amenityvm);
    }
    [HttpPost]
    public IActionResult Update(AmenityVM amenityvm)
    {

        if (ModelState.IsValid)
        {
            _unitOfWork.Amenity.Update(amenityvm.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "The Amenity has been updated successfully";
            return RedirectToAction(nameof(Index));
        }
        amenityvm.BungalowList = _unitOfWork.Bungalow.GetAll().Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        });
        return View(amenityvm);
    }
    public IActionResult Delete(int amenityId)
    {

        AmenityVM amenityvm = new()
        {
            Amenity = _unitOfWork.Amenity.Get(x => x.Id == amenityId),
            BungalowList = _unitOfWork.Bungalow.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
        };
        if (amenityvm.Amenity is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(amenityvm);
    }
    [HttpPost]
    public IActionResult Delete(AmenityVM amenityvm)
    {
        Amenity? deleteamenity = _unitOfWork.Amenity.Get(u => u.Id == amenityvm.Amenity.Id);
        if (deleteamenity is not null)
        {
            _unitOfWork.Amenity.Delete(deleteamenity);
            _unitOfWork.Save();
            TempData["success"] = "The Amenity has been deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "The Amenity could not be deleted!";

        return View();
    }
}
