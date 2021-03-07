var prevScrollpos = window.pageYOffset;
window.onscroll = function() {
  var currentScrollPos = window.pageYOffset;
  if (prevScrollpos > currentScrollPos) {
    document.getElementsByClassName("sidebarIconToggle")[0].style.top = "var(--space)";
  } else {
    document.getElementsByClassName("sidebarIconToggle")[0].style.top = "-350px";
  }
  prevScrollpos = currentScrollPos;
}