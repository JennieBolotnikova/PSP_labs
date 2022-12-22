'use strict';

// Using fs module
const fs = require('fs');

// Using Map for routing paths
const routing = new Map();
routing.set('/chat', fs.readFileSync('./client/chat.html', 'utf8'));
routing.set('/api', (req, res, callback) => {
  callback(`Api url: ${req.url}`);
});

module.exports.routing = routing;
