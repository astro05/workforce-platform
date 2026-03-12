require('dotenv').config();

const { startScheduler }   = require('./scheduler');
const { closeConnections } = require('./db');
const logger               = require('./logger');

logger.info('Report Worker starting up...');

// Start the scheduler
startScheduler();

// ── Health heartbeat every 60s ────────────────────────────
setInterval(() => {
  logger.info('[HEALTH] Report worker alive');
}, 60_000);

// ── Graceful shutdown ─────────────────────────────────────
async function shutdown(signal) {
  logger.info({ signal }, 'Shutting down report worker...');
  await closeConnections();
  process.exit(0);
}

process.on('SIGTERM', () => shutdown('SIGTERM'));
process.on('SIGINT',  () => shutdown('SIGINT'));

process.on('unhandledRejection', (reason) => {
  logger.error({ reason }, 'Unhandled rejection');
});