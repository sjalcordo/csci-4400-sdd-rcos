// Connect to the server
const socket = io();

socket.emit('update');

// Listen for updates to the lobby list from the server
socket.on('updated-players', (players) => {
    const lobbyContainer = document.getElementById('players');
    lobbyContainer.innerHTML = ''; // Clear existing player cards

    // Create a player card for each player in the list
    players.forEach(player => {
        let name = player[0];
        let base64 = player[1];
        console.log("Player " + name);

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
    });
});

socket.on('on-game-start', function(){
    window.location.href = "/prompt/prompt.html";
})