'use strict';

// Use HTTP server
const http = require('http');
// Use WebSocket
const Websocket = require('websocket').server;
// Use logger module
const logger = require('./tools/logger');
// Use router module
const router = require('./tools/router');
// Use server module
const serve = require('./tools/serve');

// Use clustering
const cluster = require('cluster');
const os = require('os');

// Process id
const pid = process.pid;

// Selecting port
const PORT = 8000;

// Clustering
if (cluster.isMaster) {
    const count = os.cpus().length;
    console.log(`Master pid: ${pid}`);
    console.log(`Starting ${count} forks`);
    for (let i = 0; i < count; i++) cluster.fork();
} 
else {
    const id = cluster.worker.id;
    console.log(`Worker: ${id}, pid: ${pid}, port: ${PORT}`);

    // Server creating
    const server = http.createServer((req, res) => {
        logger.log(req);

        res.setHeader('Process-Id', pid);
        const data = router.routing.get(req.url);
        serve.serve(data, req, res);
    });

    // Server starting
    server.listen(PORT, () => {
        console.log('Listen port ' + PORT);
    });

    // WebSocket creating
    const ws = new Websocket({
        httpServer: server,
        autoAcceptConnections: false
    });

    // Clients list
    const clients = [];

    // Using WebSocket to get requests
    ws.on('request', req => {
        // Adding client
        const connection = req.accept('', req.origin);
        clients.push(connection);
        console.log('Connected ' + connection.remoteAddress);

        // Getting client message
        connection.on('message', message => {
            const dataName = message.type + 'Data';
            const data = message[dataName];
            console.dir(message);
            console.log('Received: ' + data);
            clients.forEach(client => {
                if (connection !== client) {
                    client.send(data);
                }
            });
      });

      // On client disconnection
      connection.on('close', (reasonCode, description) => {
            console.log('Disconnected ' + connection.remoteAddress);
            console.dir({ reasonCode, description });
        });
    });
}
