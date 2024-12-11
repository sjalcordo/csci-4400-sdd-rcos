// AUTHORS: Amanda (ruana2@rpi.edu) and Nneoma (anaemn@rpi.edu)
// DESC: Implement the heart animation and proceed to following page
//       after time is up or players answer all questions.

// Connect to the server
const socket = io();

// "loading" the hearts - change the image after 0.5 seconds
const images = container.querySelectorAll('#container img');

function Animate () 
{

    // update images: empty hearts to filled hearts
    images.forEach((image, index) =>
    {
        setTimeout(() =>
        {
            image.src = "../Resources/heart-filled.png";
        }, 500 * (index + 1));
    });

    // wait until above animation is done before resetting
    setTimeout(() =>
    {
        // reset images to empty hearts
        images.forEach((image) =>
        {
            image.src = "../Resources/heart-outline.png";
        })
        Animate();
    }, 2000);
};

Animate();

//If its the client turn, change webpage to presenting 
socket.on('go-to-presentation', function()
{
    window.location.href = "/presenting/presenting.html"
});
    
socket.on('on-start-post-game', function()
{
    window.location.href = "/postGame/postGame.html";
});

socket.on('on-between-presentations', function()
{
    window.location.href = "/swipe/swipe.html";
});

socket.on('end-of-question', function()
{
    window.location.href = "/swipe/swipe.html";
});

