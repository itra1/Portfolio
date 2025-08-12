document.addEventListener("DOMContentLoaded", () => {
  disableScroll1();
  disableScroll2();

  document.body.style.overflow = "hidden";
  document.body.style.userSelect = "none";
});

function disableScroll1() {
  console.log("disableScroll");
  scrollTop =
    window.pageYOffset ||
    document.documentElement.scrollTop;
  scrollLeft =
    window.pageXOffset ||
    document.documentElement.scrollLeft,

    // if any scroll is attempted,
    // set this to the previous value
    window.onscroll = function () {
      window.scroll(0, 0);
      //window.scrollTo(scrollLeft, scrollTop);
      //return false;
    };
}

function disableScroll2() {
  window.addEventListener('DOMMouseScroll', preventDefault, false); // older FF
  window.addEventListener(wheelEvent, preventDefault, wheelOpt); // modern desktop
  window.addEventListener('touchmove', preventDefault, wheelOpt); // mobile
  window.addEventListener('keydown', preventDefaultForScrollKeys, false);
}

