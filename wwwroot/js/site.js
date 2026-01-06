// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// ========== Language switching via localStorage ==========
(function () {
    const STORAGE_KEY = 'ui-language';
    const DEFAULT_LANG = 'pl-PL';
    const SUPPORTED = ['pl-PL', 'en-US'];

    // Get saved language from localStorage (default: pl-PL)
    function getSavedLang() {
        const saved = localStorage.getItem(STORAGE_KEY);
        return SUPPORTED.includes(saved) ? saved : DEFAULT_LANG;
    }

    // Get culture from current URL query string
    function getUrlCulture() {
        const params = new URLSearchParams(window.location.search);
        return params.get('lang');
    }

    // Remove lang param from URL (clean URL after applying)
    function getCleanUrl() {
        const url = new URL(window.location.href);
        url.searchParams.delete('lang');
        return url.pathname + url.search;
    }

    // Add lang param to a URL
    function addCultureToUrl(baseUrl, culture) {
        const url = new URL(baseUrl, window.location.origin);
        url.searchParams.set('lang', culture);
        return url.pathname + url.search;
    }

    // On page load: sync localStorage with URL
    const urlCulture = getUrlCulture();
    const savedLang = getSavedLang();

    if (urlCulture && SUPPORTED.includes(urlCulture)) {
        // URL has lang param - save it to localStorage (user just switched)
        if (urlCulture !== localStorage.getItem(STORAGE_KEY)) {
            localStorage.setItem(STORAGE_KEY, urlCulture);
        }
    } else if (savedLang !== DEFAULT_LANG) {
        // No lang in URL but localStorage has non-default language - redirect with lang
        window.location.href = addCultureToUrl(window.location.href, savedLang);
    }

    // Expose function for language switch button
    window.switchLanguage = function (newLang) {
        if (!SUPPORTED.includes(newLang)) return;
        localStorage.setItem(STORAGE_KEY, newLang);
        window.location.href = addCultureToUrl(getCleanUrl(), newLang);
    };

    // Get current language (for UI display)
    window.getCurrentLanguage = function () {
        return getUrlCulture() || getSavedLang();
    };
})();
