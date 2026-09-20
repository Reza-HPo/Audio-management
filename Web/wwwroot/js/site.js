document.addEventListener("DOMContentLoaded", () => {

    "use strict";


    /* =========================================================
       CONFIG
    ========================================================= */

    const CONFIG = {
        headerSelector: ".site-header, header",
        mobileMenuSelector: ".mobile-menu",
        mobileMenuToggleSelector: ".mobile-menu-toggle",
        backToTopSelector: ".back-to-top",
        alertSelector: ".alert",
        smoothScrollOffset: 80
    };


    /* =========================================================
       BODY READY
    ========================================================= */

    document.documentElement.classList.add("js-enabled");


    /* =========================================================
       MOBILE MENU
    ========================================================= */

    const mobileToggle =
        document.querySelector(
            CONFIG.mobileMenuToggleSelector
        );

    const mobileMenu =
        document.querySelector(
            CONFIG.mobileMenuSelector
        );


    if (mobileToggle && mobileMenu) {

        mobileToggle.addEventListener(
            "click",
            () => {

                const isOpen =
                    mobileMenu.classList.toggle("is-open");

                mobileToggle.classList.toggle(
                    "is-active",
                    isOpen
                );

                mobileToggle.setAttribute(
                    "aria-expanded",
                    String(isOpen)
                );

                document.body.classList.toggle(
                    "menu-open",
                    isOpen
                );

            }
        );


        /*
         * Close menu when clicking outside.
         */

        document.addEventListener(
            "click",
            (event) => {

                if (
                    !mobileMenu.contains(event.target) &&
                    !mobileToggle.contains(event.target)
                ) {

                    mobileMenu.classList.remove(
                        "is-open"
                    );

                    mobileToggle.classList.remove(
                        "is-active"
                    );

                    mobileToggle.setAttribute(
                        "aria-expanded",
                        "false"
                    );

                    document.body.classList.remove(
                        "menu-open"
                    );

                }

            }
        );


        /*
         * Close menu after selecting a link.
         */

        mobileMenu
            .querySelectorAll("a")
            .forEach((link) => {

                link.addEventListener(
                    "click",
                    () => {

                        mobileMenu.classList.remove(
                            "is-open"
                        );

                        mobileToggle.classList.remove(
                            "is-active"
                        );

                        mobileToggle.setAttribute(
                            "aria-expanded",
                            "false"
                        );

                        document.body.classList.remove(
                            "menu-open"
                        );

                    }
                );

            });


        /*
         * Close menu with Escape.
         */

        document.addEventListener(
            "keydown",
            (event) => {

                if (event.key !== "Escape") {
                    return;
                }

                if (!mobileMenu.classList.contains("is-open")) {
                    return;
                }

                mobileMenu.classList.remove(
                    "is-open"
                );

                mobileToggle.classList.remove(
                    "is-active"
                );

                mobileToggle.setAttribute(
                    "aria-expanded",
                    "false"
                );

                document.body.classList.remove(
                    "menu-open"
                );

                mobileToggle.focus();

            }
        );

    }


    /* =========================================================
       HEADER SCROLL EFFECT
    ========================================================= */

    const header =
        document.querySelector(
            CONFIG.headerSelector
        );


    function updateHeader() {

        if (!header) {
            return;
        }


        const scrollTop =
            window.scrollY ||
            document.documentElement.scrollTop;


        header.classList.toggle(
            "is-scrolled",
            scrollTop > 20
        );

    }


    updateHeader();


    window.addEventListener(
        "scroll",
        updateHeader,
        {
            passive: true
        }
    );


    /* =========================================================
       SMOOTH ANCHOR SCROLL
    ========================================================= */

    document
        .querySelectorAll('a[href^="#"]')
        .forEach((link) => {

            link.addEventListener(
                "click",
                (event) => {

                    const href =
                        link.getAttribute("href");


                    if (
                        !href ||
                        href === "#" ||
                        href.length <= 1
                    ) {
                        return;
                    }


                    const target =
                        document.querySelector(href);


                    if (!target) {
                        return;
                    }


                    event.preventDefault();


                    const headerHeight =
                        header
                            ? header.offsetHeight
                            : CONFIG.smoothScrollOffset;


                    const targetPosition =
                        target.getBoundingClientRect().top +
                        window.scrollY -
                        headerHeight -
                        15;


                    window.scrollTo({
                        top: Math.max(
                            0,
                            targetPosition
                        ),
                        behavior: "smooth"
                    });


                    /*
                     * Update URL without jumping.
                     */

                    if (
                        history.pushState
                    ) {

                        history.pushState(
                            null,
                            "",
                            href
                        );

                    }

                }
            );

        });


    /* =========================================================
       BACK TO TOP
    ========================================================= */

    const backToTop =
        document.querySelector(
            CONFIG.backToTopSelector
        );


    if (backToTop) {

        function updateBackToTop() {

            backToTop.classList.toggle(
                "is-visible",
                window.scrollY > 500
            );

        }


        updateBackToTop();


        window.addEventListener(
            "scroll",
            updateBackToTop,
            {
                passive: true
            }
        );


        backToTop.addEventListener(
            "click",
            () => {

                window.scrollTo({
                    top: 0,
                    behavior: "smooth"
                });

            }
        );

    }


    /* =========================================================
       AUTO DISMISS ALERTS
    ========================================================= */

    document
        .querySelectorAll(CONFIG.alertSelector)
        .forEach((alert) => {

            /*
             * Only auto-dismiss alerts explicitly marked:
             *
             * data-auto-dismiss="true"
             */

            if (
                alert.dataset.autoDismiss !== "true"
            ) {
                return;
            }


            const timeout =
                Number(
                    alert.dataset.dismissAfter
                ) || 5000;


            setTimeout(
                () => {

                    alert.classList.add(
                        "is-hiding"
                    );


                    setTimeout(
                        () => {

                            alert.remove();

                        },
                        300
                    );

                },
                timeout
            );

        });


    /* =========================================================
       PASSWORD VISIBILITY
       ========================================================= */

    document
        .querySelectorAll("[data-password-toggle]")
        .forEach((button) => {

            button.addEventListener(
                "click",
                () => {

                    const targetId =
                        button.dataset.passwordToggle;


                    if (!targetId) {
                        return;
                    }


                    const input =
                        document.getElementById(
                            targetId
                        );


                    if (!input) {
                        return;
                    }


                    const isPassword =
                        input.type === "password";


                    input.type =
                        isPassword
                            ? "text"
                            : "password";


                    button.setAttribute(
                        "aria-label",
                        isPassword
                            ? "پنهان کردن رمز عبور"
                            : "نمایش رمز عبور"
                    );


                    button.classList.toggle(
                        "is-visible",
                        isPassword
                    );

                }
            );

        });


    /* =========================================================
       COPY TO CLIPBOARD
    ========================================================= */

    document
        .querySelectorAll("[data-copy]")
        .forEach((button) => {

            button.addEventListener(
                "click",
                async () => {

                    const value =
                        button.dataset.copy;


                    if (!value) {
                        return;
                    }


                    try {

                        await navigator.clipboard.writeText(
                            value
                        );


                        const originalText =
                            button.textContent;


                        button.classList.add(
                            "is-copied"
                        );


                        button.textContent =
                            "کپی شد ✓";


                        setTimeout(
                            () => {

                                button.classList.remove(
                                    "is-copied"
                                );

                                button.textContent =
                                    originalText;

                            },
                            1800
                        );

                    }
                    catch (error) {

                        console.warn(
                            "Clipboard operation failed:",
                            error
                        );

                    }

                }
            );

        });


    /* =========================================================
       LAZY IMAGE LOADING FALLBACK
    ========================================================= */

    const lazyImages =
        document.querySelectorAll(
            "img[data-src]"
        );


    if (
        lazyImages.length &&
        "IntersectionObserver" in window
    ) {

        const imageObserver =
            new IntersectionObserver(
                (entries, observer) => {

                    entries.forEach(
                        (entry) => {

                            if (!entry.isIntersecting) {
                                return;
                            }


                            const image =
                                entry.target;


                            const source =
                                image.dataset.src;


                            if (source) {

                                image.src =
                                    source;

                            }


                            image.removeAttribute(
                                "data-src"
                            );


                            image.classList.add(
                                "is-loaded"
                            );


                            observer.unobserve(
                                image
                            );

                        }
                    );

                },
                {
                    rootMargin: "200px 0px"
                }
            );


        lazyImages.forEach(
            (image) => {

                imageObserver.observe(
                    image
                );

            }
        );

    }


    /* =========================================================
       IMAGE LOAD STATE
    ========================================================= */

    document
        .querySelectorAll("img")
        .forEach((image) => {

            if (image.complete) {

                image.classList.add(
                    "is-loaded"
                );

                return;

            }


            image.addEventListener(
                "load",
                () => {

                    image.classList.add(
                        "is-loaded"
                    );

                },
                {
                    once: true
                }
            );


            image.addEventListener(
                "error",
                () => {

                    image.classList.add(
                        "is-error"
                    );

                },
                {
                    once: true
                }
            );

        });


    /* =========================================================
       FORM SUBMIT LOADING STATE
    ========================================================= */

    document
        .querySelectorAll("form")
        .forEach((form) => {

            form.addEventListener(
                "submit",
                () => {

                    /*
                     * Do not interfere with forms
                     * that explicitly disable this feature.
                     */

                    if (
                        form.dataset.loading === "false"
                    ) {
                        return;
                    }


                    form.classList.add(
                        "is-submitting"
                    );


                    const submitButtons =
                        form.querySelectorAll(
                            'button[type="submit"], input[type="submit"]'
                        );


                    submitButtons.forEach(
                        (button) => {

                            button.classList.add(
                                "is-loading"
                            );

                        }
                    );

                }
            );

        });


    /* =========================================================
       RIPPLE EFFECT
       ========================================================= */

    document
        .querySelectorAll("[data-ripple]")
        .forEach((element) => {

            element.addEventListener(
                "click",
                (event) => {

                    const rect =
                        element.getBoundingClientRect();


                    const ripple =
                        document.createElement(
                            "span"
                        );


                    const size =
                        Math.max(
                            rect.width,
                            rect.height
                        );


                    ripple.className =
                        "ripple-effect";


                    ripple.style.width =
                        `${size}px`;


                    ripple.style.height =
                        `${size}px`;


                    ripple.style.left =
                        `${event.clientX - rect.left - size / 2}px`;


                    ripple.style.top =
                        `${event.clientY - rect.top - size / 2}px`;


                    element.appendChild(
                        ripple
                    );


                    setTimeout(
                        () => {

                            ripple.remove();

                        },
                        600
                    );

                }
            );

        });


    /* =========================================================
       REDUCED MOTION
    ========================================================= */

    const prefersReducedMotion =
        window.matchMedia(
            "(prefers-reduced-motion: reduce)"
        );


    if (prefersReducedMotion.matches) {

        document.documentElement.classList.add(
            "reduced-motion"
        );

    }


    prefersReducedMotion.addEventListener?.(
        "change",
        (event) => {

            document.documentElement.classList.toggle(
                "reduced-motion",
                event.matches
            );

        }
    );


    /* =========================================================
       INITIAL PAGE STATE
    ========================================================= */

    requestAnimationFrame(
        () => {

            document.documentElement.classList.add(
                "page-ready"
            );

        }
    );

});
