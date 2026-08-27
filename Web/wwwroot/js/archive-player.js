document.addEventListener("DOMContentLoaded", function () {

    const players = document.querySelectorAll(".custom-audio-player");

    if (!players.length) {
        return;
    }


    /*
     * ============================================================
     * FORMAT TIME
     * ============================================================
     */

    function formatTime(seconds) {

        if (!Number.isFinite(seconds) || seconds < 0) {
            return "00:00";
        }

        seconds = Math.floor(seconds);

        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = seconds % 60;

        return String(minutes).padStart(2, "0")
            + ":"
            + String(remainingSeconds).padStart(2, "0");
    }


    /*
     * ============================================================
     * STOP OTHER PLAYERS
     * ============================================================
     */

    function stopOtherPlayers(currentAudio) {

        document
            .querySelectorAll(".audio-player")
            .forEach(function (otherAudio) {

                if (otherAudio === currentAudio) {
                    return;
                }

                otherAudio.pause();
                otherAudio.currentTime = 0;

                const otherPlayer =
                    otherAudio.closest(".custom-audio-player");

                if (otherPlayer) {

                    otherPlayer.classList.remove("is-playing");

                    const otherProgress =
                        otherPlayer.querySelector(".progress-range");

                    const otherCurrentTime =
                        otherPlayer.querySelector(".current-time");

                    if (otherProgress) {
                        otherProgress.value = 0;
                    }

                    if (otherCurrentTime) {
                        otherCurrentTime.textContent = "00:00";
                    }
                }
            });
    }


    /*
     * ============================================================
     * INITIALIZE PLAYERS
     * ============================================================
     */

    players.forEach(function (player) {

        const audio =
            player.querySelector(".audio-player");

        const playButton =
            player.querySelector(".player-play-button");

        const progress =
            player.querySelector(".progress-range");

        const currentTime =
            player.querySelector(".current-time");

        const totalTime =
            player.querySelector(".total-time");

        const volume =
            player.querySelector(".volume-range");

        const volumeButton =
            player.querySelector(".volume-button");


        /*
         * --------------------------------------------------------
         * SAFETY CHECK
         * --------------------------------------------------------
         */

        if (!audio || !playButton) {
            return;
        }


        /*
         * ========================================================
         * PLAY / PAUSE
         * ========================================================
         */

        playButton.addEventListener("click", function () {

            if (audio.paused) {

                stopOtherPlayers(audio);

                audio.play()
                    .catch(function (error) {

                        console.error(
                            "Audio playback error:",
                            error
                        );

                    });

            }
            else {

                audio.pause();

            }

        });


        /*
         * ========================================================
         * PLAY EVENT
         * ========================================================
         */

        audio.addEventListener("play", function () {

            player.classList.add("is-playing");

        });


        /*
         * ========================================================
         * PAUSE EVENT
         * ========================================================
         */

        audio.addEventListener("pause", function () {

            player.classList.remove("is-playing");

        });


        /*
         * ========================================================
         * LOADED METADATA
         * ========================================================
         */

        audio.addEventListener("loadedmetadata", function () {

            if (!Number.isFinite(audio.duration)) {
                return;
            }

            if (totalTime) {
                totalTime.textContent =
                    formatTime(audio.duration);
            }

        });


        /*
         * ========================================================
         * TIME UPDATE
         * ========================================================
         */

        audio.addEventListener("timeupdate", function () {

            if (!Number.isFinite(audio.duration) ||
                audio.duration <= 0) {

                return;
            }


            const percentage =
                (audio.currentTime / audio.duration) * 100;


            if (progress) {
                progress.value = percentage;
            }


            if (currentTime) {
                currentTime.textContent =
                    formatTime(audio.currentTime);
            }

        });


        /*
         * ========================================================
         * PROGRESS SEEK
         * ========================================================
         */

        if (progress) {

            progress.addEventListener("input", function () {

                if (!Number.isFinite(audio.duration) ||
                    audio.duration <= 0) {

                    return;
                }


                const percentage =
                    Number(progress.value);


                audio.currentTime =
                    (percentage / 100) * audio.duration;

            });

        }


        /*
         * ========================================================
         * VOLUME
         * ========================================================
         */

        if (volume) {

            audio.volume =
                Number(volume.value);


            volume.addEventListener("input", function () {

                audio.volume =
                    Number(volume.value);


                if (audio.muted) {
                    audio.muted = false;
                }


                if (volumeButton) {

                    volumeButton.textContent =
                        audio.volume === 0
                            ? "🔇"
                            : "🔊";

                }

            });

        }


        /*
         * ========================================================
         * MUTE / UNMUTE
         * ========================================================
         */

        if (volumeButton) {

            volumeButton.addEventListener("click", function () {

                audio.muted =
                    !audio.muted;


                volumeButton.textContent =
                    audio.muted
                        ? "🔇"
                        : "🔊";

            });

        }


        /*
         * ========================================================
         * ENDED
         * ========================================================
         */

        audio.addEventListener("ended", function () {

            player.classList.remove("is-playing");


            if (progress) {
                progress.value = 0;
            }


            if (currentTime) {
                currentTime.textContent =
                    "00:00";
            }

        });


        /*
         * ========================================================
         * ERROR
         * ========================================================
         */

        audio.addEventListener("error", function () {

            console.error(
                "Unable to load audio:",
                audio.currentSrc || audio.src
            );

        });

    });

});