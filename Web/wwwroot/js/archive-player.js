document.addEventListener("DOMContentLoaded", () => {

    "use strict";


    /* =========================================================
       CONFIG
    ========================================================= */

    const SELECTORS = {
        player: ".custom-audio-player",
        audio: ".audio-player",
        playButton: ".player-play-button",
        progress: ".progress-range",
        currentTime: ".current-time",
        totalTime: ".total-time",
        volume: ".volume-range",
        volumeButton: ".volume-button"
    };


    const players = [
        ...document.querySelectorAll(SELECTORS.player)
    ];


    if (!players.length) {
        return;
    }


    /* =========================================================
       GLOBAL STATE
    ========================================================= */

    let activeAudio = null;


    /* =========================================================
       FORMAT TIME
    ========================================================= */

    function formatTime(seconds) {

        if (
            !Number.isFinite(seconds) ||
            seconds < 0
        ) {
            return "00:00";
        }


        seconds = Math.floor(seconds);


        const hours =
            Math.floor(seconds / 3600);


        const minutes =
            Math.floor((seconds % 3600) / 60);


        const remainingSeconds =
            seconds % 60;


        if (hours > 0) {

            return (
                String(hours).padStart(2, "0")
                + ":"
                + String(minutes).padStart(2, "0")
                + ":"
                + String(remainingSeconds).padStart(2, "0")
            );

        }


        return (
            String(minutes).padStart(2, "0")
            + ":"
            + String(remainingSeconds).padStart(2, "0")
        );

    }


    /* =========================================================
       GET PLAYER ELEMENTS
    ========================================================= */

    function getElements(player) {

        return {

            audio:
                player.querySelector(SELECTORS.audio),

            playButton:
                player.querySelector(SELECTORS.playButton),

            progress:
                player.querySelector(SELECTORS.progress),

            currentTime:
                player.querySelector(SELECTORS.currentTime),

            totalTime:
                player.querySelector(SELECTORS.totalTime),

            volume:
                player.querySelector(SELECTORS.volume),

            volumeButton:
                player.querySelector(SELECTORS.volumeButton)

        };

    }


    /* =========================================================
       UPDATE PLAY BUTTON
    ========================================================= */

    function updatePlayButton(
        player,
        isPlaying
    ) {

        const button =
            player.querySelector(
                SELECTORS.playButton
            );


        if (!button) {
            return;
        }


        const title =
            button.querySelector(".play-icon");


        const pause =
            button.querySelector(".pause-icon");


        if (title) {

            title.setAttribute(
                "aria-hidden",
                isPlaying ? "true" : "false"
            );

        }


        if (pause) {

            pause.setAttribute(
                "aria-hidden",
                isPlaying ? "false" : "true"
            );

        }


        button.setAttribute(
            "aria-label",
            isPlaying
                ? "توقف پخش"
                : "پخش صوت"
        );


        button.setAttribute(
            "title",
            isPlaying
                ? "توقف"
                : "پخش"
        );

    }


    /* =========================================================
       UPDATE VOLUME ICON
    ========================================================= */

    function updateVolumeButton(
        audio,
        button
    ) {

        if (!button) {
            return;
        }


        let icon = "🔊";


        if (audio.muted || audio.volume === 0) {

            icon = "🔇";

        }
        else if (audio.volume < 0.35) {

            icon = "🔈";

        }
        else if (audio.volume < 0.7) {

            icon = "🔉";

        }


        button.innerHTML =
            `<span aria-hidden="true">${icon}</span>`;


        button.setAttribute(
            "aria-label",
            audio.muted || audio.volume === 0
                ? "فعال کردن صدا"
                : "قطع صدا"
        );

    }


    /* =========================================================
       RESET PLAYER
    ========================================================= */

    function resetPlayer(
        player,
        resetTime = true
    ) {

        const {
            audio,
            progress,
            currentTime
        } = getElements(player);


        player.classList.remove(
            "is-playing"
        );


        updatePlayButton(
            player,
            false
        );


        if (progress) {

            progress.value = 0;

        }


        if (
            currentTime &&
            resetTime
        ) {

            currentTime.textContent =
                "00:00";

        }


        if (audio) {

            audio.pause();


            if (resetTime) {

                try {

                    audio.currentTime = 0;

                }
                catch {
                    // Ignore browser-specific media errors.
                }

            }

        }

    }


    /* =========================================================
       STOP OTHER PLAYERS
    ========================================================= */

    function stopOtherPlayers(
        currentAudio
    ) {

        players.forEach(
            function (player) {

                const {
                    audio
                } = getElements(player);


                if (
                    !audio ||
                    audio === currentAudio
                ) {
                    return;
                }


                if (!audio.paused) {

                    audio.pause();

                }


                audio.currentTime = 0;


                resetPlayer(
                    player,
                    true
                );

            }
        );

    }


    /* =========================================================
       SET ACTIVE PLAYER
    ========================================================= */

    function setActivePlayer(
        audio
    ) {

        if (activeAudio === audio) {
            return;
        }


        if (activeAudio) {

            const oldPlayer =
                activeAudio.closest(
                    SELECTORS.player
                );


            if (oldPlayer) {

                resetPlayer(
                    oldPlayer,
                    true
                );

            }

        }


        activeAudio = audio;

    }


    /* =========================================================
       CLEAR ACTIVE PLAYER
    ========================================================= */

    function clearActivePlayer(
        audio
    ) {

        if (activeAudio === audio) {

            activeAudio = null;

        }

    }


    /* =========================================================
       UPDATE PROGRESS
    ========================================================= */

    function updateProgress(
        player
    ) {

        const {
            audio,
            progress,
            currentTime,
            totalTime
        } = getElements(player);


        if (!audio) {
            return;
        }


        if (
            !Number.isFinite(audio.duration) ||
            audio.duration <= 0
        ) {
            return;
        }


        const percentage =
            Math.min(
                100,
                Math.max(
                    0,
                    (audio.currentTime /
                        audio.duration) * 100
                )
            );


        if (progress) {

            progress.value =
                percentage;

        }


        if (currentTime) {

            currentTime.textContent =
                formatTime(
                    audio.currentTime
                );

        }


        if (totalTime) {

            totalTime.textContent =
                formatTime(
                    audio.duration
                );

        }

    }


    /* =========================================================
       UPDATE PROGRESS FILL
       CSS CAN USE --progress
    ========================================================= */

    function updateProgressVisual(
        player
    ) {

        const {
            progress
        } = getElements(player);


        if (!progress) {
            return;
        }


        const value =
            Number(progress.value) || 0;


        progress.style.setProperty(
            "--progress",
            `${value}%`
        );

    }


    /* =========================================================
       SEEK
    ========================================================= */

    function seek(
        audio,
        progress
    ) {

        if (
            !audio ||
            !progress ||
            !Number.isFinite(audio.duration) ||
            audio.duration <= 0
        ) {
            return;
        }


        let percentage =
            Number(progress.value);


        if (!Number.isFinite(percentage)) {

            percentage = 0;

        }


        percentage =
            Math.min(
                100,
                Math.max(
                    0,
                    percentage
                )
            );


        audio.currentTime =
            (percentage / 100) *
            audio.duration;

    }


    /* =========================================================
       VOLUME
    ========================================================= */

    function setVolume(
        audio,
        value
    ) {

        if (!audio) {
            return;
        }


        let volume =
            Number(value);


        if (!Number.isFinite(volume)) {

            volume = 1;

        }


        volume =
            Math.min(
                1,
                Math.max(
                    0,
                    volume
                )
            );


        audio.volume =
            volume;


        if (volume > 0) {

            audio.muted = false;

        }

    }


    /* =========================================================
       TOGGLE MUTE
    ========================================================= */

    function toggleMute(
        audio
    ) {

        if (!audio) {
            return;
        }


        audio.muted =
            !audio.muted;

    }


    /* =========================================================
       PLAY AUDIO
    ========================================================= */

    async function playAudio(
        player
    ) {

        const {
            audio
        } = getElements(player);


        if (!audio) {
            return;
        }


        stopOtherPlayers(
            audio
        );


        setActivePlayer(
            audio
        );


        try {

            await audio.play();

        }
        catch (error) {

            console.error(
                "Audio playback error:",
                error
            );


            player.classList.remove(
                "is-playing"
            );


            updatePlayButton(
                player,
                false
            );

        }

    }


    /* =========================================================
       PAUSE AUDIO
    ========================================================= */

    function pauseAudio(
        player
    ) {

        const {
            audio
        } = getElements(player);


        if (!audio) {
            return;
        }


        audio.pause();

    }


    /* =========================================================
       PLAY / PAUSE
    ========================================================= */

    function togglePlayback(
        player
    ) {

        const {
            audio
        } = getElements(player);


        if (!audio) {
            return;
        }


        if (audio.paused) {

            playAudio(
                player
            );

        }
        else {

            pauseAudio(
                player
            );

        }

    }


    /* =========================================================
       INITIALIZE PLAYER
    ========================================================= */

    players.forEach(
        function (player) {

            const {
                audio,
                playButton,
                progress,
                currentTime,
                totalTime,
                volume,
                volumeButton
            } = getElements(player);


            /*
             * -----------------------------------------------------
             * SAFETY
             * -----------------------------------------------------
             */

            if (!audio || !playButton) {

                return;

            }


            /*
             * -----------------------------------------------------
             * INITIAL STATE
             * -----------------------------------------------------
             */

            player.classList.remove(
                "is-playing"
            );


            updatePlayButton(
                player,
                false
            );


            /*
             * -----------------------------------------------------
             * INITIAL VOLUME
             * -----------------------------------------------------
             */

            if (volume) {

                const initialVolume =
                    Number(volume.value);


                setVolume(
                    audio,
                    Number.isFinite(
                        initialVolume
                    )
                        ? initialVolume
                        : 1
                );

            }
            else {

                audio.volume = 1;

            }


            updateVolumeButton(
                audio,
                volumeButton
            );


            /*
             * =====================================================
             * PLAY BUTTON
             * =====================================================
             */

            playButton.addEventListener(
                "click",
                function () {

                    togglePlayback(
                        player
                    );

                }
            );


            /*
             * =====================================================
             * PLAY
             * =====================================================
             */

            audio.addEventListener(
                "play",
                function () {

                    setActivePlayer(
                        audio
                    );


                    player.classList.add(
                        "is-playing"
                    );


                    updatePlayButton(
                        player,
                        true
                    );

                }
            );


            /*
             * =====================================================
             * PLAYING
             * =====================================================
             */

            audio.addEventListener(
                "playing",
                function () {

                    player.classList.add(
                        "is-playing"
                    );


                    updatePlayButton(
                        player,
                        true
                    );

                }
            );


            /*
             * =====================================================
             * PAUSE
             * =====================================================
             */

            audio.addEventListener(
                "pause",
                function () {

                    player.classList.remove(
                        "is-playing"
                    );


                    updatePlayButton(
                        player,
                        false
                    );

                }
            );


            /*
             * =====================================================
             * LOADED METADATA
             * =====================================================
             */

            audio.addEventListener(
                "loadedmetadata",
                function () {

                    if (
                        !Number.isFinite(
                            audio.duration
                        )
                    ) {
                        return;
                    }


                    if (totalTime) {

                        totalTime.textContent =
                            formatTime(
                                audio.duration
                            );

                    }


                    updateProgress(
                        player
                    );


                    updateProgressVisual(
                        player
                    );

                }
            );


            /*
             * =====================================================
             * DURATION CHANGE
             * =====================================================
             */

            audio.addEventListener(
                "durationchange",
                function () {

                    if (
                        !Number.isFinite(
                            audio.duration
                        )
                    ) {
                        return;
                    }


                    if (totalTime) {

                        totalTime.textContent =
                            formatTime(
                                audio.duration
                            );

                    }

                }
            );


            /*
             * =====================================================
             * TIME UPDATE
             * =====================================================
             */

            audio.addEventListener(
                "timeupdate",
                function () {

                    updateProgress(
                        player
                    );


                    updateProgressVisual(
                        player
                    );

                }
            );


            /*
             * =====================================================
             * PROGRESS INPUT
             * =====================================================
             */

            if (progress) {

                progress.addEventListener(
                    "input",
                    function () {

                        updateProgressVisual(
                            player
                        );


                        seek(
                            audio,
                            progress
                        );

                    }
                );


                progress.addEventListener(
                    "change",
                    function () {

                        seek(
                            audio,
                            progress
                        );

                    }
                );

            }


            /*
             * =====================================================
             * VOLUME INPUT
             * =====================================================
             */

            if (volume) {

                volume.addEventListener(
                    "input",
                    function () {

                        setVolume(
                            audio,
                            volume.value
                        );


                        updateVolumeButton(
                            audio,
                            volumeButton
                        );

                    }
                );

            }


            /*
             * =====================================================
             * MUTE
             * =====================================================
             */

            if (volumeButton) {

                volumeButton.addEventListener(
                    "click",
                    function () {

                        toggleMute(
                            audio
                        );


                        updateVolumeButton(
                            audio,
                            volumeButton
                        );

                    }
                );

            }


            /*
             * =====================================================
             * VOLUME CHANGE
             * =====================================================
             */

            audio.addEventListener(
                "volumechange",
                function () {

                    if (volume) {

                        volume.value =
                            audio.muted
                                ? 0
                                : audio.volume;

                    }


                    updateVolumeButton(
                        audio,
                        volumeButton
                    );

                }
            );


            /*
             * =====================================================
             * ENDED
             * =====================================================
             */

            audio.addEventListener(
                "ended",
                function () {

                    player.classList.remove(
                        "is-playing"
                    );


                    updatePlayButton(
                        player,
                        false
                    );


                    if (progress) {

                        progress.value =
                            0;

                    }


                    updateProgressVisual(
                        player
                    );


                    if (currentTime) {

                        currentTime.textContent =
                            "00:00";

                    }


                    clearActivePlayer(
                        audio
                    );

                }
            );


            /*
             * =====================================================
             * ERROR
             * =====================================================
             */

            audio.addEventListener(
                "error",
                function () {

                    player.classList.remove(
                        "is-playing"
                    );


                    updatePlayButton(
                        player,
                        false
                    );


                    console.error(
                        "Unable to load audio:",
                        {
                            source:
                                audio.currentSrc ||
                                audio.src,

                            error:
                                audio.error
                        }
                    );

                }
            );


            /*
             * =====================================================
             * WAITING
             * =====================================================
             */

            audio.addEventListener(
                "waiting",
                function () {

                    player.classList.add(
                        "is-buffering"
                    );

                }
            );


            /*
             * =====================================================
             * CAN PLAY
             * =====================================================
             */

            audio.addEventListener(
                "canplay",
                function () {

                    player.classList.remove(
                        "is-buffering"
                    );

                }
            );


            /*
             * =====================================================
             * STALLED
             * =====================================================
             */

            audio.addEventListener(
                "stalled",
                function () {

                    console.warn(
                        "Audio stalled:",
                        audio.currentSrc ||
                        audio.src
                    );

                }
            );


            /*
             * =====================================================
             * LOAD START
             * =====================================================
             */

            audio.addEventListener(
                "loadstart",
                function () {

                    player.classList.add(
                        "is-loading"
                    );

                }
            );


            /*
             * =====================================================
             * LOADED DATA
             * =====================================================
             */

            audio.addEventListener(
                "loadeddata",
                function () {

                    player.classList.remove(
                        "is-loading"
                    );

                }
            );

        }
    );


    /* =========================================================
       KEYBOARD CONTROL
    ========================================================= */

    document.addEventListener(
        "keydown",
        function (event) {

            const target =
                event.target;


            /*
             * Don't hijack typing fields.
             */

            if (
                target &&
                (
                    target.tagName === "INPUT" ||
                    target.tagName === "TEXTAREA" ||
                    target.tagName === "SELECT" ||
                    target.isContentEditable
                )
            ) {
                return;
            }


            /*
             * Find focused player.
             */

            const focusedPlayer =
                document.activeElement
                    ?.closest(
                        SELECTORS.player
                    );


            if (!focusedPlayer) {
                return;
            }


            const {
                audio,
                progress
            } =
                getElements(
                    focusedPlayer
                );


            if (!audio) {
                return;
            }


            /*
             * SPACE
             */

            if (
                event.code === "Space" &&
                document.activeElement?.classList.contains(
                    "player-play-button"
                )
            ) {

                event.preventDefault();

                togglePlayback(
                    focusedPlayer
                );

                return;

            }


            /*
             * LEFT / RIGHT
             */

            if (
                event.key === "ArrowLeft" ||
                event.key === "ArrowRight"
            ) {

                if (
                    !Number.isFinite(
                        audio.duration
                    )
                ) {
                    return;
                }


                event.preventDefault();


                const direction =
                    event.key === "ArrowLeft"
                        ? -1
                        : 1;


                const newTime =
                    audio.currentTime +
                    (direction * 5);


                audio.currentTime =
                    Math.min(
                        audio.duration,
                        Math.max(
                            0,
                            newTime
                        )
                    );


                if (progress) {

                    progress.value =
                        (
                            audio.currentTime /
                            audio.duration
                        ) * 100;


                    updateProgressVisual(
                        focusedPlayer
                    );

                }

            }


            /*
             * HOME
             */

            if (
                event.key === "Home"
            ) {

                event.preventDefault();

                audio.currentTime = 0;

            }


            /*
             * END
             */

            if (
                event.key === "End"
            ) {

                if (
                    Number.isFinite(
                        audio.duration
                    )
                ) {

                    event.preventDefault();

                    audio.currentTime =
                        audio.duration;

                }

            }

        }
    );

});