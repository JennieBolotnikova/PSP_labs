'use strict';
// Using types module
const types = require('./../types/types');

// Serving url requests
const serve = (data, req, res) => {
  const type = typeof data;
  if (type === 'string') return res.end(data);
  const serializer = types.types.get(type);
  serializer([data, req, res], data => serve(data, req, res));
};

module.exports.serve = serve;
