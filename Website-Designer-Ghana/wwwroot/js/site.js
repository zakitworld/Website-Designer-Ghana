// ==========================================
// WEBSITE DESIGNER IN GHANA - SITE JAVASCRIPT
// ==========================================

// Initialize on page load
window.addEventListener('DOMContentLoaded', () => {
    initSmoothScroll();
    initScrollAnimations();
    initNavbarScroll();
    initFloatingButtons();
    removeFocusOutlines();
    initCurrencyToggle();
});

// ==========================================
// REMOVE FOCUS OUTLINES
// ==========================================
function removeFocusOutlines() {
    // Remove focus from any auto-focused elements on page load
    if (document.activeElement && document.activeElement !== document.body) {
        document.activeElement.blur();
    }

    // Prevent sections from being focusable
    document.querySelectorAll('section').forEach(section => {
        section.removeAttribute('tabindex');
    });
}

// ==========================================
// SMOOTH SCROLLING
// ==========================================
function initSmoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');

            // Skip if it's just "#"
            if (href === '#') return;

            e.preventDefault();
            const target = document.querySelector(href);

            if (target) {
                // Account for fixed navbar (approx 80px)
                const offsetTop = target.offsetTop - 85;
                window.scrollTo({
                    top: offsetTop,
                    behavior: 'smooth'
                });
            }
        });
    });
}

// ==========================================
// SCROLL ANIMATIONS (INTERSECTION OBSERVER)
// ==========================================
function initScrollAnimations() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    // Create observer if IntersectionObserver is supported
    if ('IntersectionObserver' in window) {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    // Optional: stop observing once visible
                    // observer.unobserve(entry.target);
                }
            });
        }, observerOptions);

        // Observe all elements with scroll-animate class
        document.querySelectorAll('.scroll-animate').forEach(el => {
            observer.observe(el);
        });
    } else {
        // Fallback for older browsers: show all immediately
        document.querySelectorAll('.scroll-animate').forEach(el => {
            el.classList.add('visible');
        });
    }
}

// ==========================================
// NAVBAR SCROLL EFFECT
// ==========================================
function initNavbarScroll() {
    const navbar = document.querySelector('.navbar');

    if (navbar) {
        const updateNavbar = () => {
            if (window.scrollY > 50) {
                navbar.classList.add('scrolled');
                navbar.classList.remove('bg-transparent');
                navbar.classList.add('bg-dark'); // or whatever background class you want
            } else {
                navbar.classList.remove('scrolled');
                navbar.classList.add('bg-transparent');
                navbar.classList.remove('bg-dark');
            }
        };

        window.addEventListener('scroll', updateNavbar);

        // Initial check
        updateNavbar();
    }
}

// ==========================================
// UTILITY: REFRESH SCROLL ANIMATIONS
// ==========================================
window.refreshScrollAnimations = function () {
    // Re-initialize scroll animations after Blazor renders new content
    setTimeout(() => {
        initScrollAnimations();
        // Currency toggle uses event delegation, no need to re-init
    }, 100);
}

// ==========================================
// FLOATING ACTION BUTTONS
// ==========================================
function initFloatingButtons() {
    const scrollBtn = document.getElementById('scrollToTopBtn');
    const whatsappBtn = document.querySelector('.btn-whatsapp');

    if (scrollBtn || whatsappBtn) {
        const toggleButtons = () => {
            const scrolled = window.scrollY > 300;

            if (scrollBtn) {
                scrollBtn.style.display = scrolled ? 'flex' : 'none';
            }

            if (whatsappBtn) {
                whatsappBtn.style.display = scrolled ? 'flex' : 'none';
            }
        };

        window.addEventListener('scroll', toggleButtons);
        toggleButtons(); // Initial check
    }
}

// ==========================================
// BLAZOR INTEROP: SCROLL TO TOP
// ==========================================
window.scrollToTop = function () {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

// ==========================================
// CURRENCY TOGGLE (PRICING PAGE)
// ==========================================
let currencyToggleInitialized = false;

function initCurrencyToggle() {
    // Only initialize once using event delegation
    if (currencyToggleInitialized) return;
    currencyToggleInitialized = true;

    // Use event delegation to handle dynamically loaded content
    document.body.addEventListener('click', function(e) {
        // Check if clicked element is a currency button
        if (e.target.classList.contains('currency-btn') || e.target.closest('.currency-btn')) {
            const button = e.target.classList.contains('currency-btn') ? e.target : e.target.closest('.currency-btn');
            const currency = button.getAttribute('data-currency');

            if (!currency) return;

            console.log('Currency button clicked:', currency); // Debug log

            // Update active button state
            document.querySelectorAll('.currency-btn').forEach(btn => btn.classList.remove('active'));
            button.classList.add('active');

            // Update all pricing amounts
            const pricingAmounts = document.querySelectorAll('.pricing-amount');
            console.log('Found pricing amounts:', pricingAmounts.length); // Debug log

            pricingAmounts.forEach(priceElement => {
                const currencySymbol = priceElement.querySelector('.currency-symbol');
                const amount = priceElement.querySelector('.amount');

                if (currency === 'USD') {
                    currencySymbol.textContent = '$';
                    amount.textContent = priceElement.getAttribute('data-price-usd');
                } else {
                    currencySymbol.textContent = 'GH₵';
                    amount.textContent = priceElement.getAttribute('data-price-ghs');
                }
            });
        }
    });
}
