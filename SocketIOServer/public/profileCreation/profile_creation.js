var modal = document.getElementById("modal");
var libraryButton = document.getElementById("libraryButton");
var closeButton = document.getElementById("close");
var upload = document.getElementById('upload'); 
var image = document.getElementById('image');

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

// Changes the user's profile image
upload.addEventListener('change', (event) => {
    // Get the selected file
    var file = event.target.files[0];
    
    // Check if a file was selected and if it is an image
    if (file && file.type.startsWith('image/')) {
        // Create a URL for the uploaded image
        var imageUrl = URL.createObjectURL(file);
        
        // Replace the current image with the uploaded image
        image.src = imageUrl;
    } else {
        alert("Please select a valid image file.");
    }
});


