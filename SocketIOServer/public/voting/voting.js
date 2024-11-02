const buttons = document.querySelectorAll('.votingButton');
buttons.forEach((button) => {
    button.addEventListener('click', () => select(button));
})

function select(selectedButton) {
    // if the button was already selected and was clicked again, deselect button
    const status = selectedButton.dataset.selected;
    buttons.forEach((button, index) => {
        button.dataset.selected = 'false';
        button.style.background = 'transparent';
    })
    if (status == 'false') {
        selectedButton.dataset.selected = 'true';
        selectedButton.style.background = '#ffb9c1';
    }
}

const submit = document.getElementById('submit');
submit.addEventListener('click', () => submitVote());
function submitVote() {
    // get vote
    // assumes that only one vote button will be true at the same time
    let vote = "like";
    if (document.getElementById('like').dataset.selected == 'false') {
        vote = "dislike";
    };

    // if user has finished voting before others are ready to move on, display that voting was successful
    // continues to next voting page after presenter is ready to move on
    // would only need to change CSS property and user (theoretically)
    // below code displays vote submitted
    document.getElementById('voteContainer').style.display = 'none';
    submit.style.display = 'none';
    document.getElementById('confirmation').style.display = 'block';

    // send to server
}

