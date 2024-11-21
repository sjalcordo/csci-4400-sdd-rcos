// Connect to the server
const socket = io();

// Moves to presentation
socket.on('on-presentation-start', function() {
    window.location.href = "/presenting/presenting.html";
});

socket.on('on-voting-start', function() {
    window.location.href = "/voting/voting.html";
});