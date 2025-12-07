import { writable } from 'svelte/store';

// Check initial dark mode preference
const getInitialDarkMode = (): boolean => {
    if (typeof window === 'undefined') return false;

    // Check localStorage first
    const stored = localStorage.getItem('darkMode');
    if (stored !== null) {
        return stored === 'true';
    }

    // Fall back to system preference
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
};

// Initialize dark mode on the HTML element immediately
const initDarkMode = (): boolean => {
    if (typeof window === 'undefined') return false;

    const isDark = getInitialDarkMode();

    // Apply dark class immediately to prevent flash
    if (isDark) {
        document.documentElement.classList.add('dark');
    } else {
        document.documentElement.classList.remove('dark');
    }

    return isDark;
};

// Create the dark mode store with initial value
export const darkMode = writable<boolean>(initDarkMode());

// Subscribe to changes and update DOM + localStorage
darkMode.subscribe((isDark) => {
    if (typeof window === 'undefined') return;

    if (isDark) {
        document.documentElement.classList.add('dark');
        localStorage.setItem('darkMode', 'true');
    } else {
        document.documentElement.classList.remove('dark');
        localStorage.setItem('darkMode', 'false');
    }
});
