// loadingOverlay.js
const LoadingOverlay = (function () {
    let spinner;
    let overlay;

    function init() {
        // Tạo overlay element nếu chưa tồn tại
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'overlay';
            overlay.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0, 0, 0, 0.5);
                display: none;
                justify-content: center;
                align-items: center;
                z-index: 9999;
            `;
            document.body.appendChild(overlay);
        }

        // Khởi tạo spinner
        spinner = new Spinner({
            lines: 13,
            length: 28,
            width: 14,
            radius: 42,
            scale: 1,
            corners: 1,
            color: '#ffffff',
            opacity: 0.25,
            rotate: 0,
            direction: 1,
            speed: 1,
            trail: 60,
            fps: 20,
            zIndex: 2e9,
            className: 'spinner',
            top: '50%',
            left: '50%',
            shadow: false,
            hwaccel: false,
            position: 'absolute'
        });
    }

    function show() {
        if (!overlay) init();
        overlay.style.display = 'flex';
        spinner.spin(overlay);
    }

    function hide() {
        if (overlay) {
            overlay.style.display = 'none';
            spinner.stop();
        }
    }

    return {
        show: show,
        hide: hide
    };
})();

