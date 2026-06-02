(function () {
    var storageKey = 'theme';

    function applyTheme(mode) {
        var root = document.documentElement;
        if (mode === 'light') {
            root.setAttribute('data-theme', 'light');
        } else {
            root.removeAttribute('data-theme');
        }
    }

    function readStored() {
        try {
            return localStorage.getItem(storageKey);
        } catch (e) {
            return null;
        }
    }

    function persist(mode) {
        try {
            localStorage.setItem(storageKey, mode);
        } catch (e) { /* ignore */ }
    }

    window.yagotApplyTheme = applyTheme;
    window.yagotToggleTheme = function () {
        var next = document.documentElement.getAttribute('data-theme') === 'light' ? 'dark' : 'light';
        persist(next);
        applyTheme(next);
        return next;
    };

    window.yagotSyncThemeIcon = function () {
        var btn = document.getElementById('theme-toggle');
        if (!btn) return;
        var light = document.documentElement.getAttribute('data-theme') === 'light';
        btn.setAttribute('aria-pressed', light ? 'true' : 'false');
        btn.textContent = light ? '☀' : '☽';
    };

    window.yagotInitThemeControls = function () {
        var btn = document.getElementById('theme-toggle');
        if (!btn || btn.dataset.themeBound === '1') return;
        btn.dataset.themeBound = '1';
        btn.addEventListener('click', function () {
            window.yagotToggleTheme();
            window.yagotSyncThemeIcon();
        });
    };

    var stored = readStored();
    if (stored === 'light' || stored === 'dark') {
        applyTheme(stored);
    }
})();
