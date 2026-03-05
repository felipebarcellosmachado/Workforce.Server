// JS interop for Frappe Gantt chart
window.FrappeGantt = (function () {
    'use strict';

    let _gantt = null;
    let _viewMode = 'Week';

    function getLanguage() {
        const lang = document.documentElement.lang || navigator.language || 'en';
        if (lang.startsWith('pt')) return 'pt';
        if (lang.startsWith('es')) return 'es';
        if (lang.startsWith('fr')) return 'fr';
        if (lang.startsWith('de')) return 'de';
        return 'en';
    }

    function showEmpty(el, message) {
        el.innerHTML = '<div style="display:flex;justify-content:center;align-items:center;height:100%;color:var(--rz-text-disabled-color,#9e9e9e);font-size:0.95rem;">' + message + '</div>';
        _gantt = null;
    }

    function init(elementId, tasks, viewMode) {
        _viewMode = viewMode || 'Week';
        const el = document.getElementById(elementId);
        if (!el) {
            console.warn('[FrappeGantt] Container not found: #' + elementId);
            return;
        }

        el.innerHTML = '';
        _gantt = null;

        if (!tasks || tasks.length === 0) {
            showEmpty(el, '–');
            return;
        }

        try {
            _gantt = new Gantt(el, tasks, {
                view_mode: _viewMode,
                date_format: 'YYYY-MM-DD',
                readonly: true,
                language: getLanguage(),
                today_button: true,
                bar_height: 20,
                bar_corner_radius: 3,
                padding: 18
            });
        } catch (err) {
            console.error('[FrappeGantt] init error:', err);
        }
    }

    function update(tasks) {
        if (!_gantt) return;
        if (!tasks || tasks.length === 0) return;
        try {
            _gantt.refresh(tasks);
        } catch (err) {
            console.error('[FrappeGantt] update error:', err);
        }
    }

    function setViewMode(mode) {
        _viewMode = mode;
        if (!_gantt) return;
        try {
            _gantt.change_view_mode(mode);
        } catch (err) {
            console.error('[FrappeGantt] setViewMode error:', err);
        }
    }

    return { init, update, setViewMode };
})();
