//  AUTHOR: Nneoma (anaemn@rpi.edu)
//  DESC: Implement profile picture selection and send user's image and name to server.


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
var yellowPlayer = document.getElementById('yellowPlayer');
var orangePlayer = document.getElementById('orangePlayer');
var pinkPlayer = document.getElementById('pinkPlayer');
var bluePlayer = document.getElementById('bluePlayer');

var selected_image = false;
var color = "none";
var colorFile;

// Sends the profile image to the server
function SendImage(file) 
{
    console.log(file);
    const reader = new FileReader();

    reader.onload = function()
    {
        const base64 = this.result.replace(/.*base64,/, '');
        socket.emit('set-pfp', base64);
        //socket.emit('save-pfp',base64);
    };

    reader.onerror = function()
    {
        alert("Failed to read file!");
    };

    // Convert the file to a Data URL, which includes the Base64 encoding
    reader.readAsDataURL(file);
}

// open modal
libraryButton.onclick = function()
{
    modal.style.display = "block";
}

// close modal
closeButton.onclick = function()
{
    modal.style.display = "none";
}

// close modal when clicking outside
window.onclick = function(event)
{
    if (event.target == modal)
    {
        modal.style.display = "none";
    }
}

yellowPlayer.addEventListener('click', function ()
{
    image.src = '../Resources/yellow-player.png';
    modal.style.display = "none";
    selected_image = false;
    colorFile = 'yellow-player.png';
})

orangePlayer.addEventListener('click', function ()
{
    image.src = '../Resources/orange-player.png';
    modal.style.display = "none";
    selected_image = false;
    colorFile = 'orange-player.png';
})

pinkPlayer.addEventListener('click', function ()
{
    image.src = '../Resources/pink-player.png';
    modal.style.display = "none";
    selected_image = false;
    colorFile = 'pink-player.png';
})

bluePlayer.addEventListener('click', function ()
{
    image.src = '../Resources/blue-player.png';
    modal.style.display = "none";
    selected_image = false;
    colorFile = 'blue-player.png';
})

// Changes the user's profile image
upload.addEventListener('change', (event) =>
{
    var file = event.target.files[0];
    const maxSize = 500 *1024 *1024; //500MB

    if (file && file.size > maxSize)
    {
        errorMessage.textContent = "The file size exceeds 500MB. Please upload a smaller image.";
        errorMessage.style.display = "block";
        file.value = ''; 
        image.src = '../Resources/user-icon.png';
    }
    else
    {
        errorMessage.style.display = "none";
        // Check if a file was selected and if it is an image
        if (file.type.startsWith('image/'))
        {
            var imageUrl = URL.createObjectURL(file);
            image.src = imageUrl;
            image.style.display = 'block';
            selected_image = true;
        }
        else
        {
            errorMessage.textContent ="Please select a valid image file.";
            errorMessage.style.display = "block";
            file.value = ''; 
            image.src = '../Resources/user-icon.png';
            color = "none";
        }
    }

});

// Sending Profile Creation to Server
nextButton.addEventListener('click',() =>
{
    //checks if both text input and image are provided
    if (!profileName.value)
    {
        errorMessage.textContent = "You can't date without a name!";
        errorMessage.style.display = "block";
        return;
    }
    else
    {
        errorMessage.style.display = "none";
    }

    if (selected_image)
    {
        if (upload.files[0].value  === '' )
        {
            errorMessage.textContent = "Please select an image file.";
            errorMessage.style.display = "block";
            return;
        }
        else
        {
            errorMessage.style.display = "none";
            SendImage(upload.files[0]);
        }
    }
    else
    {
        if (colorFile == "none")
        {
            errorMessage.textContent = "Please select an image file.";
            errorMessage.style.display = "block";
            return;
        }
        else
        {
            errorMessage.style.display = "none";
            switch (colorFile)
            {
                case "yellow-player.png":
                    socket.emit('set-pfp', "iVBORw0KGgoAAAANSUhEUgAAAMoAAAC/CAYAAAC/ikPaAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAIZSURBVHhe7dMxAYAwEMDAhxX/dpAGXZhLBNwtUZDjua93gK3zK7BhFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBXzMLLBQERFDrGWQAAAAASUVORK5CYII=");
                    break;
                case "orange-player.png":
                    socket.emit('set-pfp', "iVBORw0KGgoAAAANSUhEUgAAAMoAAAC0CAYAAADVTbMZAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAH9SURBVHhe7dOhAYAwEMDAB8WG7D8Hghp0yQB3JhPkeO7rHWDr/ApsGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo0BgFAiMAoFRIDAKBEaBwCgQGAUCo8CvmQX3ogPBBjJcGAAAAABJRU5ErkJggg==");
                    break;
                case "pink-player.png":
                    socket.emit('set-pfp', "iVBORw0KGgoAAAANSUhEUgAAAOgAAACvCAYAAAD39JKsAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAIXSURBVHhe7dOhAcAgEMDApwuzv4Eahoi4M5kg68y+AyR9r0CQQSHMoBBmUAgzKIQZFMIMCmEGhTCDQphBIcygEGZQCDMohBkUwgwKYQaFMINCmEEhzKAQZlAIMyiEGRTCDAphBoUwg0KYQSHMoBBmUAgzKIQZFMIMCmEGhTCDQphBIcygEGZQCDMohBkUwgwKYQaFMINCmEEhzKAQZlAIMyiEGRTCDAphBoUwg0KYQSHMoBBmUAgzKIQZFMIMCmEGhTCDQphBIcygEGZQCDMohBkUwgwKYQaFMINCmEEhzKAQZlAIMyiEGRTCDAphBoUwg0KYQSHMoBBmUAgzKIQZFMIMCmEGhTCDQphBIcygEGZQCDMohBkUwgwKYQaFMINCmEEhzKAQZlAIMyiEGRTCDAphBoUwg0KYQSHMoBBmUAgzKIQZFMIMCmEGhTCDQphBIcygEGZQCDMohBkUwgwKYQaFMINCmEEhzKAQZlAIMyiEGRTCDAphBoUwg0KYQSHMoBBmUAgzKIQZFMIMCmEGhTCDQphBIcygEGZQCDMohBkUwgwKYQaFMINCmEEhzKAQZlAIMyiEGRTCDAphBoUwg0KYQSHMoBBmUAgzKIQZFMIMCmEGhTCDQphBIcygEGZQCDMohBkUwgwKYQaFMINCmEEhzKAQZlAIMyiEGRTCDAphBoUwg0KYQSHMoBBmUAgzKIQZFLJmfhAWA8oFpkmBAAAAAElFTkSuQmCC");
                    break;
                case "blue-player.png":
                    socket.emit('set-pfp', "iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAFiUAABYlAUlSJPAAAAA5SURBVDhPY7Bq+/efmnjUQMrxqIGU45FkYCsEM1gCOTCMVSExGGiQNRQzWAAFQHjUQNLwSDbw338AB3RHXaoj0sIAAAAASUVORK5CYII=");
                    break;
            }
        }
    }
    
    //sends the data to the server
    socket.emit('set-name', profileName.value);
});

// Listen for the 'profile-creation-successful' event from the server
socket.on('set-name-successful', (message) =>
{
    //When lobby doesn't exist prints that the lobby doesn't exist
    console.log('Server response:', message);
});

socket.on('set-pfp-successful', function()
{
    window.location.href = "/lobby/lobby.html";
});

/*DEBUGGING*/
// Listen for the image sent back from the server
/*to test if it works add code to html        
<p>Returned Image:</p>
<img id="returnedImage" style="display: none" width="400px" > */
socket.on('imageBack', (imageBase64) =>
{
    const returnedImage = document.getElementById('returnedImage');
    returnedImage.src = `data:image/png;base64,${imageBase64}`;
    returnedImage.style.display = 'block';  // Display the image
});

socket.on("on-removal", () =>
{
    window.location.href = "/index.html"
});