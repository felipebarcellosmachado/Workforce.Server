// JS interop for Frappe Gantt chart – supports multiple named instances
window.FrappeGantt = (function () {
    'use strict';

    // { [elementId]: Gantt }
    const _instances = {};

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
        delete _instances[el.id];
    }

    function init(elementId, tasks, viewMode) {
        const vMode = viewMode || 'Week';
        const el = document.getElementById(elementId);
        if (!el) {
            console.warn('[FrappeGantt] Container not found: #' + elementId);
            return;
        }

        el.innerHTML = '';
        delete _instances[elementId];

        if (!tasks || tasks.length === 0) {
            showEmpty(el, '–');
            return;
        }

        try {
            _instances[elementId] = new Gantt(el, tasks, {
                view_mode: vMode,
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

    function update(elementId, tasks) {
        const gantt = _instances[elementId];
        if (!gantt) return;
        if (!tasks || tasks.length === 0) return;
        try {
            gantt.refresh(tasks);
        } catch (err) {
            console.error('[FrappeGantt] update error:', err);
        }
    }

    function setViewMode(elementId, mode) {
        const gantt = _instances[elementId];
        if (!gantt) return;
        try {
            gantt.change_view_mode(mode);
        } catch (err) {
            console.error('[FrappeGantt] setViewMode error:', err);
        }
    }

    return { init, update, setViewMode };
})();
