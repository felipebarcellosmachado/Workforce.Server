// Additional app initialization (Blazor start is now handled inline in App.razor)
console.log('app.js loaded successfully');

// Register AppState interop methods
window.registerAppStateMethods = function (appStateInterop) {
    console.log('AppState methods registered');
    
    // Save state before page unload
    window.addEventListener('beforeunload', () => {
        try {
            appStateInterop.invokeMethodAsync('SaveAppState');
        } catch (error) {
            console.error('Error saving state on unload:', error);
        }
    });
};

// Get client IP address
window.getClientIP = async function () {
    try {
        const response = await fetch('https://api.ipify.org?format=json');
        const data = await response.json();
        return data.ip || '127.0.0.1';
    } catch (error) {
        console.log('Could not get client IP, using localhost');
        return '127.0.0.1';
    }
};

// Export functions
window.downloadFileFromStream = async function (fileName, contentStreamReference) {
    try {
        const arrayBuffer = await contentStreamReference.arrayBuffer();
        const blob = new Blob([arrayBuffer]);
        const url = URL.createObjectURL(blob);

        const anchorElement = document.createElement('a');
        anchorElement.href = url;
        anchorElement.download = fileName ?? '';
        anchorElement.click();
        anchorElement.remove();

        URL.revokeObjectURL(url);
    } catch (error) {
        console.error('Error downloading file:', error);
    }
};

// Export: uses XMLHttpRequest for reliable repeated downloads.
// Returns IMMEDIATELY after xhr.send() — download happens in background.
window.downloadFileFromApi = function (apiUrl, requestBodyJson) {
    console.log('[Export] Starting download from:', apiUrl);

    var xhr = new XMLHttpRequest();
    xhr.open('POST', apiUrl, true); // async = true
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.responseType = 'blob';

    xhr.onload = function () {
        console.log('[Export] XHR onload, status:', xhr.status);
        if (xhr.status === 200) {
            var blob = xhr.response;

            // Extract filename from Content-Disposition header
            var fileName = 'download';
            var disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition) {
                var match = disposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
                if (match && match[1]) {
                    fileName = match[1].replace(/['"]/g, '');
                }
            }

            console.log('[Export] Creating download for:', fileName);

            // Trigger download via hidden anchor
            var url = URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();

            // Cleanup immediately
            document.body.removeChild(a);
            URL.revokeObjectURL(url);

            console.log('[Export] Download triggered successfully');
        } else {
            console.error('[Export] Server error:', xhr.status, xhr.statusText);
        }
    };

    xhr.onerror = function () {
        console.error('[Export] Network error');
    };

    xhr.send(requestBodyJson);
    console.log('[Export] XHR.send() called, returning to Blazor...');
    // Function returns HERE immediately, XHR continues in background
};