// AUTHOR: Amanda (ruana2@rpi.edu)
// DESC: Implement the heart animation.

const images = document.querySelectorAll('#container img');

function Animate ()
{

    // have hearts jump in place
    images.forEach((image, index) =>
    {
        setTimeout(() =>
        {
            image.classList.add('jump');
        }, 500 + 100 * (index + 1));
    });

    // wait until above animation is done before resetting
    setTimeout(() =>
    {
        // remove animation class so that animations can be played again
        images.forEach((image) =>
        {
            image.classList.remove('jump');
        })

        Animate();
    }, 2000);
};

Animate();