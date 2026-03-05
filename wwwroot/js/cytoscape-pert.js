// JS interop for Cytoscape.js – PERT/CPM network diagram
window.CytoscapePert = (function () {
    'use strict';

    let _cy = null;

    const LAYOUT_OPTIONS = {
        name: 'dagre',
        rankDir: 'LR',
        rankSep: 80,
        nodeSep: 50,
        padding: 30,
        animate: false,
        nodeDimensionsIncludeLabels: true
    };

    const STYLES = [
        {
            selector: 'node',
            style: {
                'label': 'data(label)',
                'text-valign': 'center',
                'text-halign': 'center',
                'background-color': '#4f86c6',
                'color': '#ffffff',
                'font-size': '11px',
                'width': 'label',
                'height': 'label',
                'padding': '12px',
                'shape': 'rectangle',
                'text-wrap': 'wrap',
                'text-max-width': '120px',
                'border-width': '2px',
                'border-color': '#2c5f8a'
            }
        },
        {
            selector: 'node[?isComposite]',
            style: {
                'background-color': '#5c7ba8',
                'border-style': 'dashed',
                'border-color': '#3a5a80'
            }
        },
        {
            selector: 'edge',
            style: {
                'width': 2,
                'line-color': '#9e9e9e',
                'target-arrow-color': '#9e9e9e',
                'target-arrow-shape': 'triangle',
                'curve-style': 'bezier',
                'arrow-scale': 1.2
            }
        },
        {
            selector: ':selected',
            style: {
                'background-color': '#e67e22',
                'border-color': '#d35400',
                'line-color': '#e67e22',
                'target-arrow-color': '#e67e22'
            }
        }
    ];

    function init(elementId, elements) {
        const el = document.getElementById(elementId);
        if (!el) {
            console.warn('[CytoscapePert] Container not found: #' + elementId);
            return;
        }

        if (_cy) {
            _cy.destroy();
            _cy = null;
        }

        _cy = cytoscape({
            container: el,
            elements: elements || [],
            style: STYLES,
            layout: LAYOUT_OPTIONS,
            userZoomingEnabled: false,
            userPanningEnabled: true,
            minZoom: 0.15,
            maxZoom: 4.0
        });
    }

    function update(elements) {
        if (!_cy) return;
        _cy.elements().remove();
        if (elements && elements.length > 0) {
            _cy.add(elements);
        }
        _cy.layout(LAYOUT_OPTIONS).run();
    }

    function zoomIn() {
        if (!_cy) return;
        const newZoom = Math.min(_cy.zoom() * 1.25, 4.0);
        _cy.zoom({ level: newZoom, renderedPosition: { x: _cy.width() / 2, y: _cy.height() / 2 } });
    }

    function zoomOut() {
        if (!_cy) return;
        const newZoom = Math.max(_cy.zoom() / 1.25, 0.15);
        _cy.zoom({ level: newZoom, renderedPosition: { x: _cy.width() / 2, y: _cy.height() / 2 } });
    }

    function fit() {
        if (!_cy) return;
        _cy.fit(undefined, 20);
    }

    return { init, update, zoomIn, zoomOut, fit };
})();
