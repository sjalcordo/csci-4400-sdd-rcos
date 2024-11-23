
// Connect to the server
const socket = io();

var modal = document.getElementById("modal");
var libraryButton = document.getElementById("libraryButton");
var closeButton = document.getElementById("close");
var upload = document.getElementById('upload'); 
var image = document.getElementById('image');
var profile = document.getElementById('user_profile');
var profileName = document.getElementById('name');
var nextButton = document.getElementById('next');
var errorMessage = document.getElementById('errorMessage');

// Sends the profile image to the server
function sendImage() {
    const file = upload.files[0];
    const reader = new FileReader();

    reader.onload = function() {
        const base64 = this.result.replace(/.*base64,/, '');
        socket.emit('set-pfp', base64);
        //socket.emit('save-pfp',base64);
    };

    reader.onerror = function() {
        alert("Failed to read file!");
    };

    // Convert the file to a Data URL, which includes the Base64 encoding
    reader.readAsDataURL(file);
}

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
    var file = event.target.files[0];
    const maxSize = 2 *1024 *1024; //2MB

    if (file && file.size > maxSize){
        errorMessage.textContent = "The file size exceeds 2MB. Please upload a smaller image.";
        errorMessage.style.display = "block";
        file.value = ''; 
        image.src = '../Resources/user-icon.png';
    } else{
        errorMessage.style.display = "none";
        // Check if a file was selected and if it is an image
        if (file.type.startsWith('image/')) {
            var imageUrl = URL.createObjectURL(file);
            image.src = imageUrl;
            image.style.display = 'block';
        } else {
            errorMessage.textContent ="Please select a valid image file.";
            errorMessage.style.display = "block";
            file.value = ''; 
            image.src = '../Resources/user-icon.png';
        }
    }

});

// Sending Profile Creation to Server
nextButton.addEventListener('click',() =>{
    //checks if both text input and image are provided
    if (!profileName.value) {
        alert("You can't date without a name!");
        return;
    }
    if (upload.files[0].value  === '') {
        alert("Please select an image file.");
        return;
    }
    
    //sends the data to the server
    socket.emit('set-name',profileName.value);
    sendImage();
    
});

// Listen for the 'profile-creation-successful' event from the server
socket.on('set-name-successful', (message) =>{
    //When lobby doesn't exist prints that the lobby doesn't exist
    console.log('Server response:', message);
});

socket.on('set-pfp-successful', function() {
    window.location.href = "/lobby/lobby.html";
});

/*DEBUGGING*/
// Listen for the image sent back from the server
/*to test if it works add code to html        
<p>Returned Image:</p>
<img id="returnedImage" style="display: none" width="400px" > */
socket.on('imageBack', (imageBase64) => {
    const returnedImage = document.getElementById('returnedImage');
    returnedImage.src = `data:image/png;base64,${imageBase64}`;
    returnedImage.style.display = 'block';  // Display the image
});