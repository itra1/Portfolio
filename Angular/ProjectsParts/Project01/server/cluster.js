let cluster = require('cluster');

let config = {
  numWorkers: require('os').cpus().length,
};

cluster.setupMaster({
  exec: "./server.js"
});

// Fork workers as needed.
for (let i = 0; i < config.numWorkers; i++)
  cluster.fork();
