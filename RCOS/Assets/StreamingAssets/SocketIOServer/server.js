// Required constants for setting up the server and sockets
const express = require('express');
const app = express();
const http = require('http');
const server = http.createServer(app);
const { Server } = require("socket.io");
const io = new Server(server);
const fs = require("fs"); 
var path = require('path');

// Allow connected clients to use the public folder.
app.use(express.static('public'));

// Once the server receives a connection
io.on('connection', (socket) => {
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
    });
    
    // Pass through any signals except those overridden above
    socket.onAny((eventName, args) => {
        io.emit(eventName, args);
    });
});

// Open server to the 3000 port.
server.listen(3000, () => {
    console.log('listening on *:3000');
})