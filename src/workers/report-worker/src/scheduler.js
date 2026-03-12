const cron                  = require('node-cron');
const { buildDashboardReport } = require('./aggregator');
const { getMongoDb }        = require('./db');
const logger                = require('./logger');

async function saveReport(report) {
  const db = await getMongoDb();

  await db.collection('DashboardReports').replaceOne(
    { reportKey: 'dashboard' },  // filter
    report,                       // replacement
    { upsert: true }              // insert if not exists
  );

  logger.info('Dashboard report saved to MongoDB');
}

async function runReport() {
  try {
    logger.info('Report job started');
    const report = await buildDashboardReport();
    await saveReport(report);
    logger.info('Report job completed');
  } catch (err) {
    logger.error({ err }, 'Report job failed');
  }
}

function startScheduler() {
  logger.info('Scheduler started — running every 5 minutes');

  // Run immediately on startup
  runReport();

  // Then every 5 minutes
  cron.schedule('*/5 * * * *', () => {
    runReport();
  });
}

module.exports = { startScheduler };