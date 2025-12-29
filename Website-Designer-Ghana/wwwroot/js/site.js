// ==========================================
// WEBSITE DESIGNER GHANA - SITE JAVASCRIPT
// ==========================================

// Initialize on page load
window.addEventListener('DOMContentLoaded', () => {
    initSmoothScroll();
    initScrollAnimations();
    initNavbarScroll();
});

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
    }, 100);
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
