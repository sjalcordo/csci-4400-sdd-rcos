const answers = document.querySelectorAll('.answerCard');
answers.forEach((answer, index) => {
    answer.addEventListener('click', () => select(answer))
})

function select(selectedAnswer) {
    // if the answer was already selected and was clicked again, deselect answer
    const status = selectedAnswer.dataset.selected;
    answers.forEach((answer, index) => {
        answer.dataset.selected = 'false';
        answer.style.background = 'transparent';
    })
    if (status == 'false') {
        selectedAnswer.dataset.selected = 'true';
        selectedAnswer.style.background = '#ffb9c1';
    }
}

//Timer
let timeRemaining =  60; 
// Select the timer bar element
const timerBar = document.querySelector('.timerBar div');

// Set animation duration dynamically (in seconds)
const durationInSeconds = timeRemaining + 1; // there's a one sec lag
timerBar.style.animationDuration = `${durationInSeconds}s`;


// Update the timer display and progress bar every second
const updateTimer = setInterval(() => {
    const minutes = Math.floor(timeRemaining / 60);
    const seconds = timeRemaining % 60;
    document.getElementById('timer').textContent =
        `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;

    timeRemaining--;

    if (timeRemaining < 0) {
        clearInterval(updateTimer);
        document.getElementById('timer').textContent = "0:00";
    }
}, 1000);