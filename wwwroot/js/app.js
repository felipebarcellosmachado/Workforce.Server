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