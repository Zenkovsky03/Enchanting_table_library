using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Biblioteka.Services;

/// <summary>
/// Custom culture provider that reads culture from a single "lang" query string parameter.
/// </summary>
public class LangQueryStringRequestCultureProvider : RequestCultureProvider
{
    public string QueryStringKey { get; set; } = "lang";

    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var request = httpContext.Request;

        if (!request.Query.TryGetValue(QueryStringKey, out var langValues))
        {
            return NullProviderCultureResult;
        }

        var lang = langValues.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(lang))
        {
            return NullProviderCultureResult;
        }

        // Validate that it's a valid culture
        try
        {
            _ = new CultureInfo(lang);
        }
        catch (CultureNotFoundException)
        {
            return NullProviderCultureResult;
        }

        var result = new ProviderCultureResult(lang, lang);
        return Task.FromResult<ProviderCultureResult?>(result);
    }
}
