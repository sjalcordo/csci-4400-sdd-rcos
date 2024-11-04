
// Connect to the server
const socket = io();

//Timer Stuff
let timeRemaining =  30; 
// Select the timer bar element
const timerBar = document.querySelector('.timerBar div');

// Update the timer display and progress bar every second
const updateTimer = setInterval(() => {
    const minutes = Math.floor(timeRemaining / 60);
    const seconds = timeRemaining % 60;
    document.getElementById('timer').textContent = `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;

    timeRemaining--;

    if (timeRemaining < 0) {
        clearInterval(updateTimer);
        document.getElementById('timer').textContent = "0:00";
        // will refresh page to move on to the next question
        // MAYBE: A time out page? or pop up? 
        // NOTE: Maybe we should signal to the host that the player's timer ran out so
        // they know to move on to the next question
        window.location.href = "/prompt/prompt.html";
    }
}, 1000);

// Set animation duration dynamically (in seconds)
const durationInSeconds = timeRemaining + 5; // there's a 5 sec lag
timerBar.style.animationDuration = `${durationInSeconds}s`;

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
socket.emit('send-prompt');
socket.on('prompt-return',(question) =>{
    const questionContainer = document.getElementById('questionContainer');
    questionContainer.innerHTML = ''; 

    const questionElement = document.createElement('h1');
    questionElement.textContent = `Question ${question[0]}`;

    const promptElement = document.createElement('h4');
    promptElement.textContent = `${question[1]}`;

    questionContainer.appendChild(questionElement);
    questionContainer.appendChild(promptElement);
});

socket.emit('send-answer');
socket.on('answers-return', (responses) =>{
    const answersContainer = document.getElementById('answers');
    answersContainer.innerHTML = '';

    responses.forEach((response) => {
        const answerButton = document.createElement('button');
        answerButton.textContent = response;
        answerButton.className = 'answerCard';
        answerButton.dataSelected = 'false';


        // When an answer is clicked, send it to the server
        answerButton.addEventListener('click', () => {
            socket.emit('prompt-response', response);
        });

        answersContainer.appendChild(answerButton);
    })

});

socket.on('end-of-question', function() {
    //TODO: Need to create the "Let's Date Page!" Right now will just go to the voting page 
    window.location.href = "/prompt/prompt.html";
})






