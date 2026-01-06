using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Biblioteka.Controllers;

public class PreferencesController : Controller
{
    private const string ThemeCookieName = "ui-theme";

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string culture, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(culture))
        {
            return RedirectToLocal(returnUrl);
        }

        // Normalize + validate against supported cultures.
        var supported = new[] { "pl-PL", "en-US" };
        if (!supported.Contains(culture, StringComparer.OrdinalIgnoreCase))
        {
            return RedirectToLocal(returnUrl);
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(new CultureInfo(culture))),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                HttpOnly = false,
            });

        // Also include culture in the redirect URL so the next request renders in the chosen language
        // even if the browser doesn't immediately send the cookie.
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            var redirected = QueryHelpers.AddQueryString(
                returnUrl,
                new Dictionary<string, string?>
                {
                    ["culture"] = culture,
                    ["ui-culture"] = culture,
                });

            return LocalRedirect(redirected);
        }

        var homeUrl = Url.Action("Index", "Home") ?? "/";
        var redirectedHome = QueryHelpers.AddQueryString(
            homeUrl,
            new Dictionary<string, string?>
            {
                ["culture"] = culture,
                ["ui-culture"] = culture,
            });

        return Redirect(redirectedHome);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetTheme(string theme, string? returnUrl = null)
    {
        theme = (theme ?? string.Empty).Trim().ToLowerInvariant();
        if (theme != "light" && theme != "dark")
        {
            return RedirectToLocal(returnUrl);
        }

        Response.Cookies.Append(
            ThemeCookieName,
            theme,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                HttpOnly = false,
            });

        return RedirectToLocal(returnUrl);
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
