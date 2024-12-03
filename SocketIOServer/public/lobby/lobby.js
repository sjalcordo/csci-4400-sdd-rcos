// Connect to the server
const socket = io();
let firstConnect = true;

socket.on('connect', () => {
    if (!firstConnect) {
        return;
    }

    firstConnect = false;
    socket.emit('update');
});

// Listen for updates to the lobby list from the server
socket.on('updated-players', (names, b64) => {
    const lobbyContainer = document.getElementById('players');
    lobbyContainer.innerHTML = ''; 

    for(var i = 0; i < names.length; i++) {
        let name = names[i].toUpperCase() ;
        let base64 = b64[i];
        
        const playerCard = document.createElement('div');
        playerCard.className = 'playerCard';
        if (i % 2 !== 0) {
            playerCard.style.float = 'right';
            playerCard.style.clear = 'left';
        }
        
        // User image
        let imageElement = document.createElement('img');
        imageElement.src = `data:image/png;base64,${base64}`;
        imageElement.alt = `${name}'s image`;
        imageElement.className = 'userImage'

        // User name
        let nameElement = document.createElement('h3');
        nameElement.textContent = name;

        playerCard.appendChild(imageElement);
        playerCard.appendChild(nameElement);
        lobbyContainer.appendChild(playerCard);
    }
});

socket.on('on-game-start', function(){
    window.location.href = "/countdown/countdown.html";
});

socket.on("on-removal", () =>{
    window.location.href = "/index.html"
});
