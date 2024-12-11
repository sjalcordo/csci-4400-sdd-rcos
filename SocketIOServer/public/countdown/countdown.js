// AUTHORS: Amanda (ruana2@rpi.edu) and Nneoma (anaemn@rpi.edu)
// DESC: Implements the animation on the Countdown page.

// "loading" the hearts - change the image after 0.5 seconds
const number = document.getElementById('number');
const container = document.getElementById('container');
const images = container.querySelectorAll('img');

let current = Number(number.innerHTML);

function Animate (count)
{

    // update images: empty hearts to filled hearts
    images.forEach((image, index) =>
    {
        setTimeout(() =>
        {
            image.src = "../Resources/heart-filled.png";
        }, 500 * (index + 1));
    });

    // wait until above animation is done before changing num and resetting
    setTimeout(() =>
    {
        // decrease count and update html
        current = count - 1;
        if (count == 1)
        {   // stop after filling in hearts for 1
            container.innerHTML="<h1>GAME START!</h1>";
            setTimeout(() =>
            {
                window.location.href = "/prompt/prompt.html";
            }, 500);
            return;
        }
        number.innerHTML = current;

        // reset images to empty hearts
        images.forEach((image) =>
        {
            image.src = "../Resources/heart-outline.png";
        })

        Animate(current);
    }, 2000);
};

Animate(current);

