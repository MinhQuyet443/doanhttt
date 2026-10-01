function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: "smooth"
    });
}

document.addEventListener("DOMContentLoaded", function () {
    const hearts = document.querySelectorAll(".heart");

    hearts.forEach(function (btn) {
        btn.addEventListener("click", function () {
            if (btn.innerText === "♡") {
                btn.innerText = "♥";
                btn.style.color = "#ff334b";
            } else {
                btn.innerText = "♡";
                btn.style.color = "#4ba3ff";
            }
        });
    });
});