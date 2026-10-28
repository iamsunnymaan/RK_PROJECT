using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MY_PROJECT.Models;

namespace MY_PROJECT.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private const int PageSize = 10;
    private readonly AppDbContext _db;

    public CustomersController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _db.Customers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => c.Name.Contains(term) || c.Email.Contains(term)
                                     || c.Phone.Contains(term) || (c.City != null && c.City.Contains(term)));
        }

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var customers = await query
            .OrderByDescending(c => c.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Page = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.Total = total;
        return View(customers);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return customer == null ? NotFound() : View(customer);
    }

    public IActionResult Create() => View(new Customer());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        await ValidateEmailUnique(customer);
        if (!ModelState.IsValid) return View(customer);

        customer.Id = 0;
        customer.CreatedOn = DateTime.UtcNow;
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Customer '{customer.Name}' created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        return customer == null ? NotFound() : View(customer);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer model)
    {
        if (id != model.Id) return BadRequest();

        var customer = await _db.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        await ValidateEmailUnique(model);
        if (!ModelState.IsValid) return View(model);

        customer.Name = model.Name;
        customer.Email = model.Email;
        customer.Phone = model.Phone;
        customer.Address = model.Address;
        customer.City = model.City;
        customer.IsActive = model.IsActive;
        customer.UpdatedOn = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Customer '{customer.Name}' updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return customer == null ? NotFound() : View(customer);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer != null)
        {
            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Customer '{customer.Name}' deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateEmailUnique(Customer customer)
    {
        var email = customer.Email?.Trim();
        if (string.IsNullOrEmpty(email)) return;

        if (await _db.Customers.AnyAsync(c => c.Email == email && c.Id != customer.Id))
            ModelState.AddModelError(nameof(Customer.Email), "A customer with this email already exists.");
    }
}
