var modal = document.getElementById("modal");
var libraryButton = document.getElementById("libraryButton");
var closeButton = document.getElementById("close");

// open modal
libraryButton.onclick = function() {
    modal.style.display = "block";
}

// close modal
closeButton.onclick = function() {
    modal.style.display = "none";
}

// close modal when clicking outside
window.onclick = function(event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}