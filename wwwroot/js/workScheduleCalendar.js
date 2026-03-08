// JS interop for @event-calendar/core (vkurko/calendar)
// Manages resource-timeline calendar instances used in the WorkScheduleTab
window.WorkScheduleCalendar = (function () {
    const _instances = {};

    function init(elementId, options, dotNetRef) {
        const el = document.getElementById(elementId);
        if (!el) {
            console.warn('[WorkScheduleCalendar] Element not found: #' + elementId);
            return;
        }

        if (_instances[elementId]) {
            _instances[elementId].destroy();
            delete _instances[elementId];
        }

        // FIX: date-only strings (e.g. "2024-01-15") are parsed as UTC midnight by JS,
        // which in UTC-3 becomes Jan 14 local time — shifting the visible week/day range.
        const startDate = options.startDate
            ? new Date(options.startDate + 'T00:00:00')
            : new Date();

        const config = {
            // Default to month view so the full schedule is visible on first open.
            // User can switch to Week/Day from the toolbar.
            view: options.view || 'resourceTimelineMonth',
            date: startDate,
            height: options.height || '620px',
            resources: options.resources || [],
            events: options.events || [],
            editable: options.editable === true,
            locale: 'pt-br',
            firstDay: 0,
            headerToolbar: {
                start: 'prev,next today',
                center: 'title',
                end: 'dayGridMonth,resourceTimeGridDay,resourceTimeGridWeek,resourceTimelineWeek,resourceTimelineMonth'
            },
            buttonText: {
                today: 'Hoje',
                dayGridMonth: 'Grade Mês',
                resourceTimeGridDay: 'Recurso Grade Dia',
                resourceTimeGridWeek: 'Recurso Grade Semana',
                resourceTimelineWeek: 'Recurso Timeline Semana',
                resourceTimelineMonth: 'Recurso Timeline Mês'
            },
            noEventsContent: 'Nenhuma escala encontrada',
            // Smaller slot width for better day/week readability
            slotWidth: options.slotWidth || 32,
            resourceAreaWidth: '220px',
            resourceAreaColumns: [
                {
                    field: 'title',
                    headerContent: 'Horário / Funcionário'
                }
            ],
            eventClick: function (info) {
                if (!dotNetRef) return;
                const evt = info.event;
                dotNetRef.invokeMethodAsync('OnCalendarEventClicked', {
                    id: evt.id,
                    title: evt.title,
                    start: evt.start ? evt.start.toISOString() : null,
                    end: evt.end ? evt.end.toISOString() : null,
                    resourceId: evt.resourceId,
                    extendedProps: evt.extendedProps || {}
                }).catch(function (err) {
                    console.error('[WorkScheduleCalendar] Blazor callback error:', err);
                });
            }
        };

        if (options.editable) {
            config.eventDrop = function (info) {
                if (!dotNetRef) return;
                dotNetRef.invokeMethodAsync('OnCalendarEventMoved', {
                    id: info.event.id,
                    start: info.event.start ? info.event.start.toISOString() : null,
                    end: info.event.end ? info.event.end.toISOString() : null,
                    resourceId: info.event.resourceId
                }).catch(function (err) {
                    console.error('[WorkScheduleCalendar] Blazor callback error:', err);
                });
            };
        }

        try {
            _instances[elementId] = new EventCalendar(el, config);
        } catch (ex) {
            console.error('[WorkScheduleCalendar] Failed to create calendar:', ex);
        }
    }

    function setOption(elementId, option, value) {
        if (_instances[elementId]) {
            _instances[elementId].setOption(option, value);
        }
    }

    // Forces a full layout recalculation — call after the host container
    // transitions from hidden to visible (e.g. Radzen tab activated).
    function refresh(elementId) {
        const cal = _instances[elementId];
        if (!cal) return;
        try {
            const currentView = cal.getOption('view');
            cal.setOption('view', currentView);
        } catch (ex) {
            console.warn('[WorkScheduleCalendar] refresh error:', ex);
        }
    }

    function destroy(elementId) {
        if (_instances[elementId]) {
            _instances[elementId].destroy();
            delete _instances[elementId];
        }
    }

    return { init, setOption, refresh, destroy };
})();
