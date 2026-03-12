const pino = require('pino');

const logger = pino({
  level: process.env.LOG_LEVEL || 'info',
  transport: {
    target: 'pino/file',
    options: { destination: 1 }, // stdout
  },
  base: {
    service: 'report-worker',
  },
  timestamp: pino.stdTimeFunctions.isoTime,
});

module.exports = logger;