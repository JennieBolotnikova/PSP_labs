'use strict';

// Using Map for list of transformation functions for different types
const types = new Map();
types.set('object', ([data], callback) => callback(JSON.stringify(data)));
types.set('undefined', (args, callback) => callback('Page not found!'));
types.set('function', ([fn, req, res], callback) => {
  if (fn.length === 3) fn(req, res, callback);
  else callback(JSON.stringify(fn(req, res)));
});

module.exports.types = types;
