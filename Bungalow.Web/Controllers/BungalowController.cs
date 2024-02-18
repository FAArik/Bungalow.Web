using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BungalowApi.Web.Controllers;

public class BungalowController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;

    public BungalowController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {
        var bungalows = _unitOfWork.Bungalow.GetAll();
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
            if (bungalow.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(bungalow.Image.FileName);
                string imagePath = Path.Combine(_hostEnvironment.WebRootPath, @"images\BungalowImage");

                using (var filestream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                {
                    bungalow.Image.CopyTo(filestream);
                }
                bungalow.ImageUrl = @"\images\BungalowImage\" + fileName;
            }
            else
            {
                bungalow.ImageUrl = "https://placehold.co/600x400";
            }
            _unitOfWork.Bungalow.Add(bungalow);
            _unitOfWork.Save();
            TempData["success"] = "The Bungalow has been created successfully";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }
    public IActionResult Update(int bungalowId)
    {
        Bungalow bungalow = _unitOfWork.Bungalow.Get(x => x.Id == bungalowId);
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
            if (bungalow.Image!=null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(bungalow.Image.FileName);
                string imagePath = Path.Combine(_hostEnvironment.WebRootPath, @"images\BungalowImage");

                if (!string.IsNullOrEmpty(bungalow.ImageUrl))
                {
                    var oldimage = Path.Combine(_hostEnvironment.WebRootPath, bungalow.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldimage))
                    {
                        System.IO.File.Delete(oldimage);
                    }
                }
                using (var filestream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                {
                    bungalow.Image.CopyTo(filestream);
                }
                bungalow.ImageUrl = @"\images\BungalowImage\" + fileName;
            }

            _unitOfWork.Bungalow.Update(bungalow);
            _unitOfWork.Save();
            TempData["success"] = "The Bungalow has been updated successfully";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }
    public IActionResult Delete(int bungalowId)
    {
        Bungalow bungalow = _unitOfWork.Bungalow.Get(x => x.Id == bungalowId);
        if (bungalow is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(bungalow);
    }
    [HttpPost]
    public IActionResult Delete(Bungalow bungalow)
    {
        Bungalow? deleteBungalow = _unitOfWork.Bungalow.Get(u => u.Id == bungalow.Id);
        if (deleteBungalow is not null)
        {
            if (!string.IsNullOrEmpty(deleteBungalow.ImageUrl))
            {
                var oldimage = Path.Combine(_hostEnvironment.WebRootPath, deleteBungalow.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldimage))
                {
                    System.IO.File.Delete(oldimage);
                }
            }
            _unitOfWork.Bungalow.Delete(deleteBungalow);
            _unitOfWork.Save();
            TempData["success"] = "The Bungalow has been deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "The Bungalow could not be deleted!";

        return View();
    }
}
