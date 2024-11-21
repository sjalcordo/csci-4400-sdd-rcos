// "loading" the hearts - change the image after 0.5 seconds
const number = document.querySelector('h1');
const images = document.querySelectorAll('img');
const container = document.getElementById('container');
let current = Number(number.innerHTML);

function animate (count) {

    // update images: empty hearts to filled hearts
    images.forEach((image, index) => {
        setTimeout(() => {
            image.src = "../Resources/heart-filled.png";
        }, 500 * (index + 1));
    });

    // wait until above animation is done before changing num and resetting
    setTimeout(() => {
        // decrease count and update html
        current = count - 1;
        if (count == 1) {   // stop after filling in hearts for 1
            container.innerHTML="<h1>GAME START!</h1>";
            setTimeout(() => {
                window.location.href = "/prompt/prompt.html";
            }, 500);
            return;
        }
        number.innerHTML = current;

        // reset images to empty hearts
        images.forEach((image) => {
            image.src = "../Resources/heart-outline.png";
        })

        animate(current);
    }, 2000);
};

animate(current);

