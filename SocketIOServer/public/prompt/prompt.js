// Connect to the server
const socket = io(); 
let firstConnect = true;
let setDuration = 3; 
const timeBar = document.getElementById('timeBar');

const answers = document.querySelectorAll('.answerCard');
answers.forEach((answer, index) => {
    answer.addEventListener('click', () => select(answer))
})

function resetTimer(time){
    console.log("setDuration = " + setDuration);
    console.log("Current Time = " + time);

    const percentage = (time / setDuration) * 100;
    timeBar.style.width = percentage + '%';

    const minutes = Math.floor(time / 60);
    const seconds = Math.ceil(time % 60);
    document.getElementById('timer').textContent = `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
}


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


socket.on('connect', () => {
    if (!firstConnect) {
        return;
    }

    firstConnect = false;
    socket.emit('request-prompt');
    socket.emit('request-answers');
});


socket.on('on-send-prompt',(question, num) =>{
    const questionContainer = document.getElementById('questionContainer');
    questionContainer.innerHTML = ''; 

    const questionElement = document.createElement('h1');
    questionElement.textContent = `Question ` + num;

    const promptElement = document.createElement('h4');
    promptElement.textContent = `${question}`;
    promptElement.className = 'prompt';

    questionContainer.appendChild(questionElement);
    questionContainer.appendChild(promptElement);
});

socket.on('on-send-answers', (responses) =>{
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
            resetTimer();
        });
        
        answersContainer.appendChild(answerButton);
    })

});

socket.on('get-setDuration', (time) =>{
    setDuration = time;
})

socket.on('on-timer-update', (time) => {
    resetTimer(time);
});

socket.on('on-timeout', function() {
    resetTimer(setDuration);
    location.reload();
});

socket.on('end-of-question', function() {
    window.location.href = "/swipe/swipe.html";
});

socket.on('on-move-to-waiting', function() {
    window.location.href = "/waitingForPlayers/waiting.html";
});



