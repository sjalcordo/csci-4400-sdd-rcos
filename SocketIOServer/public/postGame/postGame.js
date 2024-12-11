// AUTHORS: Amanda (ruana2@rpi.edu) and Sean (alcors@rpi.edu)
// DESC: Implement the confetti animation for the post game page,
//       as well as retrieve the user ranking.


// Confetti animation
window.onload = (event) =>
{
    confetti(
        {
        particleCount: 150,
        spread: 200,
        gravity: 0.5,
        origin: {x: 0, y: 0.8}
    });
    confetti(
        {
        particleCount: 150,
        spread: 200,
        gravity: 0.5,
        origin: {x: 1, y: 0.8}
    });
};

// Connect to the server
const socket = io();

var rankText = document.getElementById('rank');

let firstConnect = true;

function SetRank(rank)
{
    lastDigit = rank % 10;
    suffix = "th";
    switch (lastDigit)
    {
        case 1:
            suffix = "st";
            break;
        case 2:
            suffix = "nd";
            break;
        case 3:
            suffix = "rd";
            break;
    }
    rankText.textContent = "You won " + rank + suffix + " place!";
}

socket.on('connect', () =>
{
    if (!firstConnect)
    {
        return;
    }

    firstConnect = false;
    socket.emit("get-rank");
});

socket.on("on-send-rank", (rank) =>
{
    SetRank(rank);
});