// Connect to the server
const socket = io(); 
let firstConnect = true;
let setDuration = 20;
let updateTime = setDuration; 
const timeBar = document.getElementById('timeBar');
const inputContainer = document.getElementById('textboxForm');
const answersContainer = document.getElementById('answers');
const backButton = document.getElementById('back');
const submitButton = document.getElementById('submit');
const answers = document.querySelectorAll('.answerCard');
const inputAnswer = document.getElementById('textbox');
const profilePic = document.getElementById('profilePic');

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


backButton.addEventListener('click', function () {
    console.log("back pressed")
    answersContainer.style.display = "flex";
    inputContainer.style.display = "none";
});

submitButton.addEventListener('click', function () {
    console.log(inputAnswer.value);
    if (inputAnswer.value === ''){
        alert('Please enter a message!');
        return;
    }
    console.log("submited")
    answersContainer.style.display = "block";
    inputContainer.style.display = "none";
    socket.emit("used-fill-in");
    socket.emit('prompt-response', inputAnswer.value);
    resetTimer();
});

socket.on('connect', () => {
    if (!firstConnect) {
        return;
    }

    firstConnect = false;
    socket.emit('request-prompt');
    socket.emit('request-answers');
    socket.emit('request-player-b64');
    socket.emit('get-timer-duration');
});

socket.on('on-send-player-b64', (b64) => {
    profilePic.src = `data:image/png;base64,${b64}`;
})

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
        if (response != ""){
            const answerButton = document.createElement('button');
            answerButton.textContent = response;
            answerButton.className = 'answerCard';

        // When an answer is clicked, send it to the server
            answerButton.addEventListener('click', () => {
                socket.emit('prompt-response', response);
                console.log("here?");
                resetTimer();
            });
            answersContainer.appendChild(answerButton);   
        } else {
            const blankAnswerButton = document.createElement('button');
            blankAnswerButton.textContent = "Type in an Answer!"
            blankAnswerButton.className = 'answerCard';

            blankAnswerButton.addEventListener('click', () => {
                answersContainer.style.display = "none";
                inputContainer.style.display = "block";
            })
            answersContainer.appendChild(blankAnswerButton);   
        }
    })
});

socket.on('on-send-timer-duration', (time) =>{
    setDuration = time;
    updateTime = setDuration; 
    console.log(setDuration);
})

socket.on('on-timer-update', (time) => {
    updateTime = updateTime - 1;
    resetTimer(updateTime);
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

//DEBUGGING
function createAnswers(){
    answersContainer.innerHTML = '';

    responses = ["Love it! Sweet and salty, just like me!",
                "Only if I’m trying to impress my taste buds.",
                "It’s an unforgivable sin, honestly.",
                "If you like it, we are not compatible.", ""] 

    responses.forEach((response) => {
        if (response != ""){
            const answerButton = document.createElement('button');
            answerButton.textContent = response;
            answerButton.className = 'answerCard';

        // When an answer is clicked, send it to the server
            answerButton.addEventListener('click', () => {
                socket.emit('prompt-response', response);
                console.log("here?");
                resetTimer();
            });
            answersContainer.appendChild(answerButton);   
        } else {
            const blankAnswerButton = document.createElement('button');
            blankAnswerButton.textContent = "Type in an Answer!"
            blankAnswerButton.className = 'answerCard';

            blankAnswerButton.addEventListener('click', () => {
                answersContainer.style.display = "none";
                inputContainer.style.display = "block";
            })
            answersContainer.appendChild(blankAnswerButton);   
        }
        
    })
}

//createAnswers();

