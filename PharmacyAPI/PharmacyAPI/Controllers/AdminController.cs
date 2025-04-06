using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyAPI.Data;
using PharmacyAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    private async Task<IActionResult> CheckDuplicateEmail(string email, bool isPharmacy)
    {
        if (isPharmacy)
        {
            if (await _context.Pharmacy.AnyAsync(p => p.Email == email))
                return BadRequest(new { message = "Email already exists for pharmacy." });
        }
        else
        {
            if (await _context.Suppliers.AnyAsync(s => s.Email == email))
                return BadRequest(new { message = "Email already exists for supplier." });
        }
        return null;
    }

    [HttpPost("add-pharmacy")]
    public async Task<IActionResult> AddPharmacy([FromBody] Pharmacy pharmacy)
    {
        if (pharmacy == null)
            return BadRequest(new { message = "Invalid pharmacy data" });

        var duplicateCheck = await CheckDuplicateEmail(pharmacy.Email, true);
        if (duplicateCheck != null)
            return duplicateCheck;

        _context.Pharmacy.Add(pharmacy);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Pharmacy added successfully" });
    }

    [HttpPost("add-supplier")]
    public async Task<IActionResult> AddSupplier([FromBody] Supplier supplier)
    {
        if (supplier == null)
            return BadRequest(new { message = "Invalid supplier data" });

        var duplicateCheck = await CheckDuplicateEmail(supplier.Email, false);
        if (duplicateCheck != null)
            return duplicateCheck;

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Supplier added successfully" });
    }

   
}
