using BungalowApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

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
        return View();
    }
}
