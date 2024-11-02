const buttons = document.querySelectorAll('.votingButton');
buttons.forEach((button) => {
    button.addEventListener('click', () => select(button));
})

function select(selectedButton) {
    // if the button was already selected and was clicked again, deselect button
    const status = selectedButton.dataset.selected;
    buttons.forEach((button) => {
        button.dataset.selected = 'false';
        button.style.background = 'transparent';
    })
    if (status == 'false') {
        selectedButton.dataset.selected = 'true';
        selectedButton.style.background = '#ffb9c1';
    }
}

const submit = document.getElementById('submit');
const confirmation = document.getElementById('confirmation');
let timeout = 0; // reference to timeout call
submit.addEventListener('click', () => submitVote());
document.addEventListener('keydown', (event) => {
    if (event.key === "Enter") {
        submitVote();
    }
})
function submitVote() {
    // get vote
    // assumes that only one vote button will be true at the same time
    let vote = "like";
    if (document.getElementById('like').dataset.selected == 'false') {
        vote = "dislike";
    };

    // send to server

    // display vote submission confirmation
    if (confirmation.classList.contains('fadeOut')){
        // if vote is submitted while confirmation is still displayed, restart confirmation message
        confirmation.classList.remove('fadeOut');
        confirmation.offsetHeight;  // triggers reflow and does the line above before continuing
        confirmation.classList.add('fadeOut');
        clearTimeout(timeout);  // remove previous timeout
    }
    else {
        // display confirmation
        confirmation.classList.remove('hidden');
        confirmation.classList.add('fadeOut');
    }
    timeout = window.setTimeout(() => {
        confirmation.classList.add('hidden');
        confirmation.classList.remove('fadeOut');
    }, 3000)

    // reset buttons
    buttons.forEach((button) => {
        button.dataset.selected = 'false';
        button.style.background = 'transparent';
    })
}

