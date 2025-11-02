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

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    window.themeManager = new ThemeManager();
    window.pwaInstaller = new PWAInstaller();

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