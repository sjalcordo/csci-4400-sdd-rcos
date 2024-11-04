// "loading" the hearts - change the image after 0.5 seconds
const images = document.querySelectorAll('img');

function animate () {

    // update images: empty hearts to filled hearts
    images.forEach((image, index) => {
        setTimeout(() => {
            image.src = "../Resources/heart-filled.png";
        }, 500 * (index + 1));
    });

    // wait until above animation is done before resetting
    setTimeout(() => {
        // reset images to empty hearts
        images.forEach((image) => {
            image.src = "../Resources/heart-outline.png";
        })

        animate();
    }, 2000);
};

animate();

//Waits till server alerts to go to next question
socket.on('all-answered', function(){
    window.location.href = "/prompt/prompt.html";
});

//When all the questions have been answered, lets go to the voting page
socket.on('to-voting-page', function(){
    window.location.href = "/voting/voting.html"
})

//