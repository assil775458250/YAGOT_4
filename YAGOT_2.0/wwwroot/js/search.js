(function () {
    var minChars = 2;
    var debounceMs = 280;

    function debounce(fn, ms) {
        var t;
        return function () {
            var ctx = this, args = arguments;
            clearTimeout(t);
            t = setTimeout(function () { fn.apply(ctx, args); }, ms);
        };
    }

    function renderList(box, items) {
        box.innerHTML = '';
        if (!items || !items.length) {
            box.hidden = true;
            return;
        }
        box.hidden = false;
        items.forEach(function (p) {
            var a = document.createElement('a');
            a.className = 'nav-search-suggestion';
            a.href = '/Products/Details/' + encodeURIComponent(p.id);
            var img = document.createElement('img');
            img.alt = '';
            img.src = p.imageUrl || '/images/placeholder-product.svg';
            img.onerror = function () { this.src = '/images/placeholder-product.svg'; };
            var span = document.createElement('span');
            span.textContent = p.name;
            a.appendChild(img);
            a.appendChild(span);
            box.appendChild(a);
        });
    }

    window.yagotInitLiveSearch = function () {
        var input = document.getElementById('nav-search-q');
        var box = document.getElementById('nav-search-suggestions');
        if (!input || !box) return;

        var run = debounce(function () {
            var q = (input.value || '').trim();
            if (q.length < minChars) {
                renderList(box, []);
                return;
            }
            fetch('/api/search/suggestions?q=' + encodeURIComponent(q), { headers: { 'Accept': 'application/json' } })
                .then(function (r) { return r.ok ? r.json() : []; })
                .then(function (data) { renderList(box, data); })
                .catch(function () { renderList(box, []); });
        }, debounceMs);

        input.addEventListener('input', run);
        input.addEventListener('focus', run);
        document.addEventListener('click', function (e) {
            if (!e.target.closest('.nav-search-wrap')) {
                box.hidden = true;
            }
        });
    };
})();
