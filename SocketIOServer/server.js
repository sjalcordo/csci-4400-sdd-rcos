/*
Server script used to host an Express server to serve files at a URL at the 3000 port using Socket.IO as a data communication line.
*/

// Helper class that is used to organize servers.
class Lobby {
    host;
    players = {};

    constructor(hostSocket) {
        this.players = {};
        this.host = hostSocket;
    }
}

// Helper class that contains everything needed to store a player.
class Player {
    name = "";
    socket;

    constructor(socket) {
        this.socket = socket;
    }
}

// Required constants for setting up the server and sockets
const express = require('express');
const app = express();
const http = require('http');
const server = http.createServer(app);
const { Server } = require("socket.io");
const io = new Server(server);
const fs = require("fs"); 

// Used for hashing a connected player's IP
var md5 = require('js-md5');

var bannedServerNames = ["tests"];
// Key: <string> hashedIP
// Value: <Lobby>
let lobbyDict = {};
// Key: <string> hashedIP
// Value: <string> lobbyID
let cachedPlayers = {};

// Allow connected clients to use the public folder.
app.use(express.static('public'));

// Once the server receives a connection
io.on('connection', (socket) => {
    let ipAddress = socket.handshake.address;
    let hashedIP = md5.hmac('RCOS', ipAddress);
    let lobbyID = "";
    
    if (hashedIP in cachedPlayers) {
        joinLobby(socket, cachedPlayers[hashedIP], hashedIP);
        lobbyID = cachedPlayers[hashedIP];
    }

    // If the socket has "UNITY" as the token, recognize the client as the Unity client.
    if(socket.handshake.query.token === "UNITY") {
        console.log("Unity Socket connected");
    }
    else {
        console.log("Browser Connected");
    }

    // On disconnect, keep track of if a browser or the Unity client disconnects.
    // If the Unity browser disconnects, then clear the demo list.
    socket.on('disconnect', function() {
        if(socket.handshake.query.token === "UNITY") {
            console.log("Unity Socket disconnected");
        } 
        else {
            console.log("Browser disconnected");
        }
        
        // If the cached lobbyID no longer exists in the dictionary, short-circuit. 
        if (!(lobbyID in lobbyDict)){
            return;
        }

        // Removes the player socket from the server
        if (lobbyID != null && lobbyID != "" && lobbyDict[lobbyID].players != null) {
            delete lobbyDict[lobbyID].players[hashedIP];
        }
        if (lobbyID in lobbyDict && lobbyDict[lobbyID].host == socket) {
            delete lobbyDict[lobbyID];
        }
    });

    /* 
    LOBBY CREATION 
    */
    // Creates a lobby and sends a verification signal back to the requesting client.
    socket.on('create-lobby', function() {
        // Generate lobby ID
        lobbyID = GenerateLobbyName(lobbyDict);
        
        lobbyDict[lobbyID] = new Lobby(socket);
        socket.emit('verify-lobby', lobbyID);
    });

    // Joins an existing lobby
    socket.on('join-lobby', (lobby) => {      
        joinLobby(socket,lobby, hashedIP);

        lobbyID = lobby;
        cachedPlayers[hashedIP] = lobby;
    });

    // Destroys a lobby if it currently exists.
    socket.on('destroy-lobby', (lobby) => {
        if (!lobby in lobbyDict) 
            return;

        delete lobbyDict[lobby];
    });
    
    /* 
    PROFILE CREATION 
    */
    // Sets a players name and sends the hashedIP of the player and the name to the hosting client.
    socket.on('set-name', (name) => {
        if (lobbyID == "" || !lobbyID in lobbyDict || !hashedIP in lobbyDict[lobbyID].players) {
            return;
        }

        lobbyDict[lobbyID].players[hashedIP].name = name;
        lobbyDict[lobbyID].host.emit("on-set-name", hashedIP, name);
    });

    // Receives an image in base64 format and sends that string to the hosting client.
    socket.on('set-pfp', (base64) => {
        if (lobbyID == "" || !lobbyID in lobbyDict || !hashedIP in lobbyDict[lobbyID].players)
            return;

        lobbyDict[lobbyID].host.emit('on-set-pfp', hashedIP, base64);
        socket.emit('set-pfp-successful',"Image has been successfully saved into server");
    });

    /*
    PROMPTS
    */
    // Called when the game flow begins.
    socket.on('game-start', function(){
        if (lobbyID == "" || !(lobbyID in lobbyDict)) {
            return;
        }
        
        // Sends the signal to each connected player of the lobby
        Object.entries(lobbyDict[lobbyID].players).forEach(([key, value]) => {
            value.socket.emit("on-game-start");
        });
    });

    // Requests a prompt from the host.
    socket.on('request-prompt', function() {
        if (lobbyID == "")
            return;
        lobbyDict[lobbyID].host.emit("on-request-prompt", hashedIP);
    });

    // Sends a prompt to a player via hashedIP.
    socket.on('send-prompt', (hashedIP, prompt, num) => {
       if (lobbyID == "") 
            return;

        lobbyDict[lobbyID].players[hashedIP].socket.emit('on-send-prompt', prompt, num);
    });

    // Called when the client requests the answers for a prompt.
    socket.on('request-answers', function() {
        if (lobbyID == "")
            return;
        lobbyDict[lobbyID].host.emit("on-request-answers", hashedIP);
    });

    // Sends a client their answers from the host.
    socket.on('send-answers', (hashedIP, answers) => {
        if (lobbyID == "") 
             return;
 
         lobbyDict[lobbyID].players[hashedIP].socket.emit('on-send-answers', answers);
    });

    // Sends the response to a prompt, tied to the hashedIP.
    socket.on('prompt-response', (response) => {
       if (lobbyID == "") 
            return;

        lobbyDict[lobbyID].host.emit('on-prompt-response', hashedIP, response); 
        console.log("User press: " + response);
    });

    // Unsure if you want a different signal for blankprompt response
    socket.on('blankprompt-response', (response) => {
        console.log("User input: " + response);
    })

    socket.on('send-answer', function(){
        console.log("Sending over answers");
        socket.emit('answers-return', answers);
    });

    socket.on('timer-update', (hashedIP, time) => {
        if (lobbyID == "" || !lobbyID in lobbyDict || !hashedIP in lobbyDict[lobbyID].players || lobbyDict[lobbyID].players[hashedIP].socket == null)
            return;

        lobbyDict[lobbyID].players[hashedIP].socket.emit('on-timer-update', time);
    });

    socket.on('time-out', hashedIP => {
        if (lobbyID == "" || !lobbyID in lobbyDict || !hashedIP in lobbyDict[lobbyID].players)
            return;

        lobbyDict[lobbyID].players[hashedIP].socket.emit('on-time-out');
    });

    socket.on('get-setDuration', (hashedIP, duration) =>{
        if (lobbyID == "" || !lobbyID in lobbyDict || !hashedIP in lobbyDict[lobbyID].players || lobbyDict[lobbyID].players[hashedIP].socket == null)
            return;

        lobbyDict[lobbyID].players[hashedIP].socket.emit('on-setDuration', duration);
    })

    /* 
    VOTING
    */
    socket.on('upvote', function() {
        if (lobbyID == "")
            return;

        lobbyDict[lobbyID].host.emit('on-upvote', hashedIP);
    });
    socket.on('downvote', function() {
        if (lobbyID == "")
            return;

        lobbyDict[lobbyID].host.emit('on-downvote', hashedIP);
    });

    socket.on("get-presenter-name", function() {
        if (lobbyID == "")
            return;

        lobbyDict[lobbyID].host.emit('on-get-presenter-name', hashedIP);
    });

    socket.on('send-presenter-name', (user, username) => {
        if (lobbyID == "" || !lobbyID in lobbyDict || !user in lobbyDict[lobbyID].players || lobbyDict[lobbyID].players[user].socket == null)
            return;

        lobbyDict[lobbyID].players[user].socket.emit('on-send-presenter-name', username);
    });
    /*
    LOBBY SCREEN
    */

    socket.on('update', ()=> {
        if (lobbyID == "" || !(lobbyID in lobbyDict)) {
            return;
        }

        lobbyDict[lobbyID].host.emit('request-player-info', hashedIP);
    });

    socket.on('on-request-player-info', (names, b64) => {
        if (lobbyID == "" || !(lobbyID in lobbyDict)) {
            return;
        }

        Object.entries(lobbyDict[lobbyID].players).forEach(([key, value]) => {
            value.socket.emit("updated-players", names, b64);
        });
    });
    
    socket.on('questions-finished', user => {
        if (lobbyID == "") 
             return;

        lobbyDict[lobbyID].players[user].socket.emit('end-of-question');
    });

    socket.on('move-to-waiting', user => {
        if (lobbyID == "") 
             return;

        Object.entries(lobbyDict[lobbyID].players).forEach(([key, value]) => {
            if (key == user) {
                value.socket.emit("on-move-to-waiting");
            }
        });
    });

    socket.on('move-all-to-waiting', function() {
        if (lobbyID == "") 
             return;

        Object.entries(lobbyDict[lobbyID].players).forEach(([key, value]) => {
            value.socket.emit("on-move-to-waiting");
        });
    });

    /*
    PRESENTATION
    */

    socket.on('between-presentations', function() {
        if (lobbyID == "" || !lobbyID in lobbyDict)
            return;

        Object.entries(lobbyDict[lobbyID].players).forEach(([key, value]) => {
            value.socket.emit("on-between-presentations");
        });
    });

    socket.on("next-presentation-slide", function()  {
        if (lobbyID == "")
            return;

        lobbyDict[lobbyID].host.emit('on-next-presentation-slide');

    });

    socket.on("presentation-start", (user) => {
        if (lobbyID == "" || !lobbyID in lobbyDict || !user in lobbyDict[lobbyID].players || lobbyDict[lobbyID].players[user].socket == null)
            return;

        lobbyDict[lobbyID].players[user].socket.emit('on-presentation-start');
    });

    socket.on('voting-start', (user) => {
        if (lobbyID == "" || !lobbyID in lobbyDict || !user in lobbyDict[lobbyID].players || lobbyDict[lobbyID].players[user].socket == null)
            return;

        Object.entries(lobbyDict[lobbyID].players).forEach(([key, value]) => {
            if (key != user){
                value.socket.emit("on-voting-start");
            }
        });
    });
 
    socket.on('presentations-finished', user => {
        if (lobbyID == "") 
             return;

        lobbyDict[lobbyID].players[user].socket.emit('on-presentations-finished');
    });

    /*
    POST GAME
    */
    socket.on('start-post-game', () => {
        if (lobbyID == "" || !lobbyID in lobbyDict)
            return;

        Object.entries(lobbyDict[lobbyID].players).forEach(([key, value]) => {
            value.socket.emit("on-start-post-game");
        });
    });
 
    socket.on("get-rank", function() {
       if (lobbyID == "") 
        return;
    
       lobbyDict[lobbyID].host.emit('on-get-rank', hashedIP);
    });

    socket.on("send-rank", (user, rank) => {
        if (lobbyID == "" || !lobbyID in lobbyDict || !user in lobbyDict[lobbyID].players || lobbyDict[lobbyID].players[user].socket == null)
            return;

        lobbyDict[lobbyID].players[user].socket.emit('on-send-rank', rank);
    });

    /* 
    DEBUGGING
    */

    // Debug to show messages in console.
    socket.onAny((eventName, args) => {
        if (args == null) {
            args = "{null}";
        }
       console.log("Received Message\n\tEventName: " + eventName + "\n\tArgs: " + args);
    });  
});

function makeid(length) {
    let result = '';
    const characters = 'abcdefghijklmnopqrstuvwxyz';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
      counter += 1;
    }
    return result;
}

function joinLobby(socket, lobby, hashedIP) {
    // If the lobby does not exist
    if (!(lobby in lobbyDict)) {
        socket.emit('join-lobby-fail-dne');
        delete cachedPlayers[hashedIP];
        return;
    }

    // Limit the amount of players
    if (lobbyDict[lobby] != null && Object.keys(lobbyDict[lobby].players).length >= 8) {
        socket.emit('join-lobby-fail-plimit')
        return;
    }

    // If the player's hashedIP is already in the players list
    if (hashedIP in lobbyDict[lobby].players) {
        lobbyDict[lobby].players[hashedIP].socket = socket;
    }
    // Otherwise, create a new player
    else {
        lobbyDict[lobby].players[hashedIP] = new Player(socket);
        lobbyDict[lobby].host.emit('new-player', hashedIP);
        console.log("STARTING NEW PLAYER");
    }
    
    socket.emit('join-lobby-success', lobby);
}

function GenerateLobbyName(lobbyDictionary) {
    let lobbyName = makeid(5);
    while (bannedServerNames.includes(lobbyName) || lobbyName in lobbyDictionary) {
        lobbyName = makeid(5);
    }
    return lobbyName;
}

// Open server to the 3000 port.
server.listen(2800, () => {
    console.log('RC:OS Server listening on *:3000');
});
