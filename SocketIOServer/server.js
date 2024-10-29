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

var bannedServerNames = ["tests"]
let lobbyDict = {};

class Lobby {
    host;
    players = {};

    constructor(hostSocket) {
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

        // Removes the player socket from the server
        if (lobbyID != null && lobbyID != "") {
            delete lobbyDict[lobbyID].players[hashedIP];
        }
    });

    // Tested and works.
    socket.on('create-lobby', function() {
        // Generate lobby ID
        lobbyName = makeid(5);
        while (bannedServerNames.includes(lobbyName)) {
            lobbyName = makeid(5);
        }

        socket.emit('verify-lobby', lobbyName);
        lobbyDict[lobbyName] = new Lobby(socket);
        console.log(lobbyDict);
    });


    // Tested and works.
    socket.on('join-lobby', (lobby) => {
        // If the lobby does not exist
        if (!lobby in lobbyDict) 
            return;

        // Limit the amount of players
        if (Object.keys(lobbyDict[lobby].players).length >= 8) 
            return;

        // If the player's hashedIP is already in the players list
        if (hashedIP in lobbyDict[lobby].players) {
            lobbyDict[lobby].players[hashedIP].socket = socket;
        }
        // Otherwise, create a new player
        else {
            lobbyDict[lobby].players[hashedIP] = new Player(socket);
        }

        console.log(lobbyDict);
        console.log(lobbyDict[lobby].players[hashedIP]);
        lobbyID = lobby;
    });

    // Tested and works.
    socket.on('destroy-lobby', (lobby) => {
        if (!lobby in lobbyDict) 
            return;

        delete lobbyDict[lobby];
    });

    // Tested and works.
    socket.on('set-name', (name) => {
        if (lobbyID == "" || !lobbyID in lobbyDict || !hashedIP in lobbyDict[lobbyID].players)
            return;

        lobbyDict[lobbyID].players[hashedIP].name = name;
    });

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

    // Debug to show messages in console.
    socket.onAny((eventName, args) => {
        if (args == null) {
            args = "{null}";
        }
        console.log("Received Message\n\tEventName: " + eventName + "\n\tArgs: " + args);
    });
});

// Open server to the 3000 port.
server.listen(3000, () => {
    console.log('listening on *:3000');
})

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