// Connect to the server
const socket = io();

var nextButton = document.getElementById('next');

// Sending Profile Creation to Server
nextButton.addEventListener('click',() =>{
    //sends the data to the server
    socket.emit('next-presentation-slide');
});

socket.on('on-between-presentations', function() {
    window.location.href = "/swipe/swipe.html";
});