// Required constants for setting up the server and sockets
const express = require('express');
const app = express();
const http = require('http');
const server = http.createServer(app);
const { Server } = require("socket.io");
const io = new Server(server);
const fs = require("fs"); 
var path = require('path');
var md5 = require('js-md5');

var bannedServerNames = ["tests"];
let lobbyDict = {};
let cachedPlayers = {};

class Lobby {
    host;
    players = {};

    constructor(hostSocket) {
        this.players = {};
        this.host = hostSocket;
    }
}

class Player {
    name = "";
    socket;

    constructor(socket) {
        this.socket = socket;
    }
}


// Allow connected clients to use the public folder.
app.use(express.static('public'));

// Once the server receives a connection
io.on('connection', (socket) => {
    let ipAddress = socket.handshake.address;
    let hashedIP = md5.hmac('RCOS', ipAddress);
    let lobbyID = "";
    
    if (hashedIP in cachedPlayers) {
        joinLobby(socket, cachedPlayers[hashedIP], hashedIP);
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
    // Tested and works.
    socket.on('disconnect', function() {
        if(socket.handshake.query.token === "UNITY") {
            console.log("Unity Socket disconnected");
        } 
        else {
            console.log("Browser disconnected");
        }

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
    
    // Tested and works.
    socket.on('create-lobby', function() {
        // Generate lobby ID
        lobbyName = makeid(5);
        while (bannedServerNames.includes(lobbyName) || lobbyName in lobbyDict) {
            lobbyName = makeid(5);
        }

        lobbyDict[lobbyName] = new Lobby(socket);
        console.log(lobbyDict[lobbyName]);
        socket.emit('verify-lobby', lobbyName);
    });


    // Tested and works.
    socket.on('join-lobby', (lobby) => {      
        joinLobby(socket,lobby, hashedIP);

        lobbyID = lobby;
        cachedPlayers[hashedIP] = lobby;
    });

    // Tested and works.
    socket.on('destroy-lobby', (lobby) => {
        if (!lobby in lobbyDict) 
            return;

        delete lobbyDict[lobby];
    });
    
    /* 
    PROFILE CREATION 
    */

    // Tested and works.
    socket.on('set-name', (name) => {
        if (lobbyID == "" || !lobbyID in lobbyDict || !hashedIP in lobbyDict[lobbyID].players)
            return;

        lobbyDict[lobbyID].players[hashedIP].name = name;
        lobbyDict[lobbyID].host.emit("set-name", hashedIP, name);
    });

    socket.on('prompt-response', (response) => {
        if (lobbyID == "") 
            return;

        lobbyDict[lobbyID].host.emit('on-prompt-response', hashedIP, response); 
    });

    /* 
    VOTING
    */

    // Tested and works.
    socket.on('upvote', function() {
        if (lobbyID == "")
            return;

        lobbyDict[lobbyID].host.emit('on-upvote', hashedIP);
    });

    // Tested and works.
    socket.on('downvote', function() {
        if (lobbyID == "")
            return;

        lobbyDict[lobbyID].host.emit('on-downvote', hashedIP);
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

    // Prints out the submitCode given from the client 
    socket.on('submitCode', (codeValues) => {
        let code = codeValues.join('');
        console.log('Received code:', code);

        //TODO: Return if the code of the lobby is correct, for now assumes any code would work
        let lobbyConnection = "Connecting to Lobby";

        //Sends a message to the client
        socket.emit('lobbyConnection', lobbyConnection);

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

    // For testing purposes, I'm creating an lobby with a code name "11111"
    //so I'm able to work on creating connections between the server and client
    if(lobby == "11111" && !(lobby in lobbyDict)){
        lobbyDict["11111"] =  new Lobby(socket);
    }

    // If the lobby does not exist
    if (!(lobby in lobbyDict)) {
        socket.emit('join-lobby-fail-dne');
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
    }
    
    socket.emit('join-lobby-success', lobby);
    lobbyDict[lobby].host.emit('new-player', hashedIP);
}

// Open server to the 3000 port.
server.listen(2000, () => {
    console.log('listening on *:3000');
})

