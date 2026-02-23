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

        const startDate = options.startDate ? new Date(options.startDate) : new Date();

        const config = {
            view: options.view || 'resourceTimelineWeek',
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
                end: 'resourceTimelineDay,resourceTimelineWeek,resourceTimelineMonth'
            },
            buttonText: {
                today: 'Hoje',
                resourceTimelineDay: 'Dia',
                resourceTimelineWeek: 'Semana',
                resourceTimelineMonth: 'Mês'
            },
            noEventsContent: 'Nenhuma escala encontrada',
            slotWidth: options.slotWidth || 72,
            resourceAreaWidth: '220px',
            resourceAreaColumns: [
                {
                    field: 'title',
                    headerContent: 'Horário / Funcionário'
                }
            ],
            slotMinTime: '00:00:00',
            slotMaxTime: '24:00:00',
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

    function destroy(elementId) {
        if (_instances[elementId]) {
            _instances[elementId].destroy();
            delete _instances[elementId];
        }
    }

    return { init: init, setOption: setOption, destroy: destroy };
})();
