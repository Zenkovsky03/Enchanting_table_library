using Biblioteka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q = null)
    {
        var users = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            users = users.Where(u =>
                (u.Email != null && u.Email.Contains(term)) ||
                (u.UserName != null && u.UserName.Contains(term)));
        }

        var list = users.OrderBy(u => u.Email ?? u.UserName).ToList();

        var rows = new List<UserRowVm>(list.Count);
        foreach (var u in list)
        {
            var roles = await _userManager.GetRolesAsync(u);
            rows.Add(new UserRowVm
            {
                Id = u.Id,
                Email = u.Email ?? u.UserName ?? "(brak)",
                IsApproved = u.IsApproved,
                IsActive = u.IsActive,
                Roles = roles.OrderBy(r => r).ToList(),
            });
        }

        return View(rows);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.IsApproved = true;
        user.IsActive = true;
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // "Odrzuć" traktujemy jako zablokowanie konta czytelnika.
        user.IsApproved = false;
        user.IsActive = false;
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return BadRequest("Nie można zablokować konta Admin.");
        }

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEmployee(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return BadRequest("Nie można zmieniać roli Admin.");
        }

        var isEmployee = await _userManager.IsInRoleAsync(user, "Employee");
        if (isEmployee)
        {
            await _userManager.RemoveFromRoleAsync(user, "Employee");
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "Employee");
        }

        return RedirectToAction(nameof(Index));
    }

    public class UserRowVm
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();

        public bool IsReader => Roles.Contains("Reader");
        public bool IsEmployee => Roles.Contains("Employee");
        public bool IsAdmin => Roles.Contains("Admin");
    }
}
