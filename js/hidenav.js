window.onscroll = function() {
    var scrollPos = window.pageYOffset;
    if (scrollPos > 50) {
        document.getElementsByClassName("navbar")[0].style.background = "linear-gradient(90deg, var(--gradient1) 0%, transparent 100%) fixed;";
    }
    else {
        document.getElementsByClassName("navbar")[0].style.background = "transparent;";
    }
}