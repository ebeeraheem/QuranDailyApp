// Theme management
class ThemeManager {
    constructor() {
        this.currentTheme = localStorage.getItem('theme') || 'system';
        this.applyTheme();
        this.setupMediaQuery();
    }

    applyTheme() {
        const root = document.documentElement;

        if (this.currentTheme === 'system') {
            const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
            root.setAttribute('data-theme', prefersDark ? 'dark' : 'light');
        } else {
            root.setAttribute('data-theme', this.currentTheme);
        }
    }

    setupMediaQuery() {
        const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
        mediaQuery.addEventListener('change', () => {
            if (this.currentTheme === 'system') {
                this.applyTheme();
            }
        });
    }

    setTheme(theme) {
        this.currentTheme = theme;
        localStorage.setItem('theme', theme);
        this.applyTheme();

        // Update active theme button
        document.querySelectorAll('.theme-option').forEach(btn => {
            btn.classList.remove('active');
        });
        document.querySelector(`[data-theme-option="${theme}"]`)?.classList.add('active');
    }

    getCurrentTheme() {
        return this.currentTheme;
    }
}

// PWA Install functionality
class PWAInstaller {
    constructor() {
        this.deferredPrompt = null;
        this.setupInstallPrompt();
    }

    setupInstallPrompt() {
        window.addEventListener('beforeinstallprompt', (e) => {
            e.preventDefault();
            this.deferredPrompt = e;
            this.showInstallPrompt();
        });

        window.addEventListener('appinstalled', () => {
            this.hideInstallPrompt();
            this.deferredPrompt = null;
        });
    }

    showInstallPrompt() {
        const existingPrompt = document.getElementById('install-prompt');
        if (existingPrompt) return;

        const installPrompt = document.createElement('div');
        installPrompt.id = 'install-prompt';
        installPrompt.className = 'install-prompt';
        installPrompt.innerHTML = `
            <div>
                <strong>Install Quran Daily</strong>
                <div style="font-size: 0.875rem; opacity: 0.9;">Get quick access to daily Quran verses</div>
            </div>
            <div>
                <button onclick="pwaInstaller.install()" style="margin-right: 0.5rem;">Install</button>
                <button onclick="pwaInstaller.dismissPrompt()">Not Now</button>
            </div>
        `;

        document.body.appendChild(installPrompt);

        // Show with animation
        setTimeout(() => {
            installPrompt.classList.add('show');
        }, 100);

        // Auto-hide after 10 seconds
        setTimeout(() => {
            this.dismissPrompt();
        }, 10000);
    }

    async install() {
        if (!this.deferredPrompt) return;

        this.deferredPrompt.prompt();
        const result = await this.deferredPrompt.userChoice;

        if (result.outcome === 'accepted') {
            console.log('User accepted the install prompt');
        } else {
            console.log('User dismissed the install prompt');
        }

        this.deferredPrompt = null;
        this.hideInstallPrompt();
    }

    dismissPrompt() {
        this.hideInstallPrompt();
    }

    hideInstallPrompt() {
        const installPrompt = document.getElementById('install-prompt');
        if (installPrompt) {
            installPrompt.classList.remove('show');
            setTimeout(() => {
                installPrompt.remove();
            }, 300);
        }
    }

    // Method to manually trigger install prompt
    triggerInstallPrompt() {
        if (this.deferredPrompt) {
            this.install();
        } else {
            // Show manual install instructions
            this.showManualInstallInstructions();
        }
    }

    showManualInstallInstructions() {
        const instructions = document.createElement('div');
        instructions.className = 'install-prompt show';
        instructions.innerHTML = `
            <div>
                <strong>Install Quran Daily</strong>
                <div style="font-size: 0.875rem; opacity: 0.9;">
                    To install: Use your browser's menu and select "Install" or "Add to Home Screen"
                </div>
            </div>
            <button onclick="this.parentElement.remove()">Got it</button>
        `;

        document.body.appendChild(instructions);

        setTimeout(() => {
            instructions.remove();
        }, 5000);
    }
}

// Toast notification system
class ToastManager {
    constructor() {
        this.createToastContainer();
    }

    createToastContainer() {
        if (document.getElementById('toast-container')) return;

        const container = document.createElement('div');
        container.id = 'toast-container';
        container.style.cssText = `
            position: fixed;
            top: 1rem;
            right: 1rem;
            z-index: 9999;
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
            pointer-events: none;
        `;
        document.body.appendChild(container);
    }

    showToast(message, type = 'success', duration = 4000) {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;

        const colors = {
            success: { bg: '#10b981', border: '#059669' },
            info: { bg: '#3b82f6', border: '#2563eb' },
            warning: { bg: '#f59e0b', border: '#d97706' },
            error: { bg: '#ef4444', border: '#dc2626' }
        };

        const color = colors[type] || colors.success;

        toast.style.cssText = `
            background: ${color.bg};
            color: white;
            padding: 0.875rem 1.25rem;
            border-radius: 0.75rem;
            border-left: 4px solid ${color.border};
            box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
            font-size: 0.875rem;
            font-weight: 500;
            max-width: 300px;
            opacity: 0;
            transform: translateX(100%);
            transition: all 0.3s ease;
            pointer-events: auto;
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        `;

        // Add appropriate icon
        const icons = {
            success: '✓',
            info: 'ℹ',
            warning: '⚠',
            error: '✕'
        };

        toast.innerHTML = `
            <span style="font-size: 1rem;">${icons[type] || icons.success}</span>
            <span>${message}</span>
        `;

        const container = document.getElementById('toast-container');
        container.appendChild(toast);

        // Trigger animation
        setTimeout(() => {
            toast.style.opacity = '1';
            toast.style.transform = 'translateX(0)';
        }, 100);

        // Auto-dismiss
        const timeoutId = setTimeout(() => {
            this.dismissToast(toast);
        }, duration);

        // Click to dismiss
        toast.addEventListener('click', () => {
            clearTimeout(timeoutId);
            this.dismissToast(toast);
        });

        return toast;
    }

    dismissToast(toast) {
        toast.style.opacity = '0';
        toast.style.transform = 'translateX(100%)';

        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }

    success(message, duration) {
        return this.showToast(message, 'success', duration);
    }

    info(message, duration) {
        return this.showToast(message, 'info', duration);
    }

    warning(message, duration) {
        return this.showToast(message, 'warning', duration);
    }

    error(message, duration) {
        return this.showToast(message, 'error', duration);
    }
}

// Date formatting utility
window.getFormattedLocalDate = function () {
    const now = new Date();
    const options = {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    };
    return now.toLocaleDateString('en-US', options);
};

// Toast notification functions for Blazor interop
window.showToast = function (message, type = 'success', duration = 4000) {
    window.toastManager.showToast(message, type, duration);
};

window.showSuccessToast = function (message, duration = 4000) {
    window.toastManager.success(message, duration);
};

window.showInfoToast = function (message, duration = 4000) {
    window.toastManager.info(message, duration);
};

window.showWarningToast = function (message, duration = 4000) {
    window.toastManager.warning(message, duration);
};

window.showErrorToast = function (message, duration = 4000) {
    window.toastManager.error(message, duration);
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    window.themeManager = new ThemeManager();
    window.pwaInstaller = new PWAInstaller();
    window.toastManager = new ToastManager();

    // Initialize theme buttons
    document.querySelectorAll('.theme-option').forEach(btn => {
        btn.classList.remove('active');
    });
    document.querySelector(`[data-theme-option="${window.themeManager.getCurrentTheme()}"]`)?.classList.add('active');
});

// Expose functions globally for Blazor interop
window.setTheme = function (theme) {
    window.themeManager.setTheme(theme);
};

window.showInstallPrompt = function () {
    window.pwaInstaller.triggerInstallPrompt();
};

window.getCurrentTheme = function () {
    return window.themeManager.getCurrentTheme();
};
