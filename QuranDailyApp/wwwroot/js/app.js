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

        // Prevent body scroll
        document.body.classList.add('modal-open');

        const installPrompt = document.createElement('div');
        installPrompt.id = 'install-prompt';
        installPrompt.className = 'install-prompt auto-install';
        installPrompt.innerHTML = `
            <h3>Install Quran Daily</h3>
            <p>Get quick access to daily Quran verses with our app. Install it on your device for offline access and a better experience.</p>
            <div>
                <button onclick="pwaInstaller.install()" class="install-btn">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
                        <polyline points="7,10 12,15 17,10"/>
                        <line x1="12" y1="15" x2="12" y2="3"/>
                    </svg>
                    Install App
                </button>
                <button onclick="pwaInstaller.dismissPrompt()" class="dismiss-btn">Not Now</button>
            </div>
        `;

        document.body.appendChild(installPrompt);

        // Show with animation
        setTimeout(() => {
            installPrompt.classList.add('show');
        }, 100);

        // Auto-hide after 15 seconds
        setTimeout(() => {
            this.dismissPrompt();
        }, 15000);

        // Close on backdrop click
        installPrompt.addEventListener('click', (e) => {
            if (e.target === installPrompt) {
                this.dismissPrompt();
            }
        });

        // Close on Escape key
        const handleEscape = (e) => {
            if (e.key === 'Escape') {
                this.dismissPrompt();
                document.removeEventListener('keydown', handleEscape);
            }
        };
        document.addEventListener('keydown', handleEscape);
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
            document.body.classList.remove('modal-open');
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
        const existingPrompt = document.getElementById('install-prompt');
        if (existingPrompt) return;

        // Prevent body scroll
        document.body.classList.add('modal-open');

        const instructions = document.createElement('div');
        instructions.id = 'install-prompt';
        instructions.className = 'install-prompt manual-install show';
        instructions.innerHTML = `
            <h3>Install Quran Daily</h3>
            <p>To install this app on your device:</p>
            <div style="text-align: left; margin: 1rem 0;">
                <strong>On Mobile:</strong><br>
                • Tap the browser menu (⋮ or share button)<br>
                • Select "Add to Home Screen" or "Install"<br><br>
                <strong>On Desktop:</strong><br>
                • Click the install icon in your browser's address bar<br>
                • Or use your browser's menu and select "Install"
            </div>
            <div>
                <button onclick="this.parentElement.parentElement.remove(); document.body.classList.remove('modal-open');" class="dismiss-btn">Got it</button>
            </div>
        `;

        document.body.appendChild(instructions);

        // Close on backdrop click
        instructions.addEventListener('click', (e) => {
            if (e.target === instructions) {
                instructions.remove();
                document.body.classList.remove('modal-open');
            }
        });

        // Close on Escape key
        const handleEscape = (e) => {
            if (e.key === 'Escape') {
                instructions.remove();
                document.body.classList.remove('modal-open');
                document.removeEventListener('keydown', handleEscape);
            }
        };
        document.addEventListener('keydown', handleEscape);

        // Auto-remove after 15 seconds
        setTimeout(() => {
            if (instructions.parentNode) {
                instructions.remove();
                document.body.classList.remove('modal-open');
            }
        }, 15000);
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
