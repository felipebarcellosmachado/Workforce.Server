window.stateManager = {
    save: function (key, str) {
        try {
            localStorage.setItem(key, str);
            console.log('State saved to localStorage:', key);
        } catch (error) {
            console.error('Error saving state:', error);
        }
    },
    load: function (key) {
        try {
            const value = localStorage.getItem(key);
            if (value) {
                console.log('State loaded from localStorage:', key);
            }
            return value;
        } catch (error) {
            console.error('Error loading state:', error);
            return null;
        }
    },
    remove: function (key) {
        try {
            localStorage.removeItem(key);
            console.log('State removed from localStorage:', key);
        } catch (error) {
            console.error('Error removing state:', error);
        }
    },
    clear: function () {
        try {
            localStorage.clear();
            console.log('All state cleared from localStorage');
        } catch (error) {
            console.error('Error clearing state:', error);
        }
    }
};