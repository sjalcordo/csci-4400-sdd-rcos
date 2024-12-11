// AUTHORS: Nneoma (anaemn@rpi.edu) and Sean (alcors@rpi.edu)
// DESC: Process and send the user's vote to the server.

// Connect to the server
const socket = io();

let firstConnect = true;

var like = document.getElementById("like");
var dislike = document.getElementById("dislike");

socket.on('connect', () =>
{
    if (!firstConnect)
    {
        return;
    }

    firstConnect = false;
    socket.emit("get-presenter-name");
});

//Changes player's name on screen
socket.on('on-send-presenter-name', (player) =>
{
    var UserName = document.getElementById("name");
    UserName.textContent = player;
});

like.addEventListener('click',() =>
{
    socket.emit('upvote');
});

dislike.addEventListener('click',() =>
{
    socket.emit('downvote');
});

socket.on('on-move-to-waiting', function()
{
    window.location.href = "/waitingForPlayers/waiting.html";
});
