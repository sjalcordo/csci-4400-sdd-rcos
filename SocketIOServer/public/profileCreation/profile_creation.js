
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
    // Get the selected file
    var file = event.target.files[0];
    
    // Check if a file was selected and if it is an image
    if (file && file.type.startsWith('image/')) {
        // Create a URL for the uploaded image
        var imageUrl = URL.createObjectURL(file);
        
        // Replace the current image with the uploaded image
        image.src = imageUrl;
        image.style.display = 'block';
    } else {
        alert("Please select a valid image file.");
    }
});

// Sending Profile Creation to Server
nextButton.addEventListener('click',() =>{
    //checks if both text input and image are provided
    if (!profileName.value) {
        alert("You can't date without a name!");
        return;
    }
    if (upload.files.length === 0) {
        alert("Please select an image file.");
        return;
    }
    
    //sends the data to the server
    socket.emit('set-name',profileName.value);

    console.log(profileName.value);
    sendImage();
    
    /*
    //clear data
    profileName.value = '';
    image.src = '../Resources/user-icon.png';
    */
});

// Listen for the 'profile-creation-successful' event from the server
socket.on('set-name-successful', (message) =>{
    //When lobby doesn't exist prints that the lobby doesn't exist
    console.log('Server response:', message);
});

socket.onAny((eventName, args) => {
    if (args == null) {
        args = "{null}";
    }
   console.log("Received Message\n\tEventName: " + eventName + "\n\tArgs: " + args);
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