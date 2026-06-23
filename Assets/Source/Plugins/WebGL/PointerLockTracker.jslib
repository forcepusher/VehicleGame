mergeInto(LibraryManager.library, {
    PointerLockTracker_Init: function () {
        var state = window.pointerLockTrackerState;
        if (!state) {
            state = {
                initialized: false,
                blockedUntil: 0,
                blockDurationMs: 2000,
            };
            window.pointerLockTrackerState = state;
        }

        if (state.initialized)
            return;

        state.initialized = true;

        function blockReacquire() {
            state.blockedUntil = performance.now() + state.blockDurationMs;
        }

        document.addEventListener('pointerlockchange', function () {
            if (document.pointerLockElement === null)
                blockReacquire();
        });

        document.addEventListener('pointerlockerror', function () {
            blockReacquire();
        });

        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape')
                blockReacquire();
        }, true);
    },

    PointerLockTracker_CanRequestLock: function () {
        var state = window.pointerLockTrackerState;
        if (!state)
            return 1;

        return performance.now() >= state.blockedUntil ? 1 : 0;
    },
});
