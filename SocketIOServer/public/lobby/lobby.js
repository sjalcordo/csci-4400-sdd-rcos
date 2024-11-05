// Connect to the server
const socket = io();

socket.emit('update');

// Listen for updates to the lobby list from the server
socket.on('updated-players', (names, b64) => {
    const lobbyContainer = document.getElementById('players');
    lobbyContainer.innerHTML = ''; // Clear existing player cards

    for(var i = 0; i < names.length; i++) {
        let name = names[i];
        let base64 = b64[i];
        
        const playerCard = document.createElement('div');
        playerCard.className = 'playerCard';
        
        // User image
        let imageElement = document.createElement('img');
        imageElement.src = `data:image/png;base64,${base64}`;
        imageElement.alt = `${name}'s image`;
        imageElement.className = 'userImage'
        imageElement.style.width = '200px'; 

        // User name
        let nameElement = document.createElement('h3');
        nameElement.textContent = name;

        // Append elements to the player card
        playerCard.appendChild(imageElement);
        playerCard.appendChild(nameElement);
        
        // Append the player card to the lobby container
        lobbyContainer.appendChild(playerCard);
    }
});

socket.on('on-game-start', function(){
    window.location.href = "/countdown/countdown.html";
});
