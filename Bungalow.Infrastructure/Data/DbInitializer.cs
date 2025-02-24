using BungalowApi.Application.Common.Utility;
using BungalowApi.Application.Services.Interface;
using BungalowApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BungalowApi.Infrastructure.Data;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public DbInitializer(ApplicationDbContext context, RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public void Initialize()
    {
        try
        {
            if (_context.Database.GetPendingMigrations().Count() > 0)
            {
                _context.Database.Migrate();
            }

            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();
                
                 _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@faarik.com",
                        Name = "FAArik",
                        NormalizedUserName = "FAARIK",
                        NormalizedEmail = "ADMIN@FAARIK.COM",
                        PhoneNumber = "0123456789",
                        EmailConfirmed = true
                    }, "Admin123*"
                ).GetAwaiter().GetResult();
                ApplicationUser usr = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@faarik.com");
                _userManager.AddToRoleAsync(usr, SD.Role_Admin).GetAwaiter().GetResult();
            }

            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}