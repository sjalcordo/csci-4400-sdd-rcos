// Connect to the server
const socket = io();

var rankText = document.getElementById('rank');

let firstConnect = false;

function SetRank(rank) {
    lastDigit = rank % 10;
    suffix = "th";
    switch (lastDigit) {
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

socket.on('connect', () => {
    if (!firstConnect) {
        return;
    }

    firstConnect = false;
    socket.emit("get-rank");
});

socket.on("on-send-rank", (rank) => {
    SetRank(rank);
});