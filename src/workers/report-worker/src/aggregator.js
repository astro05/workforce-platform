'use strict';

const { getSqlPool, getMongoDb } = require('./db');
const logger = require('./logger');

// ── Headcount Stats from SQL Server ───────────────────────
async function getHeadcountStats() {
  const pool = await getSqlPool();

  const totalResult = await pool.request().query(`
    SELECT
      COUNT(*)                                              AS Total,
      SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END)        AS Active,
      SUM(CASE WHEN IsActive = 0 THEN 1 ELSE 0 END)        AS Inactive
    FROM Employees
  `);

  const { Total, Active, Inactive } = totalResult.recordset[0];

  const deptResult = await pool.request().query(`
    SELECT
      d.Name        AS Department,
      COUNT(e.Id)   AS Count
    FROM Departments d
    LEFT JOIN Employees e
      ON e.DepartmentId = d.Id AND e.IsActive = 1
    GROUP BY d.Name
    ORDER BY Count DESC
  `);

  return {
    total:        Total,
    active:       Active,
    inactive:     Inactive,
    byDepartment: deptResult.recordset.map(r => ({
      department: r.Department,
      count:      r.Count,
    })),
  };
}

// ── Project Stats from SQL Server ─────────────────────────
async function getProjectStats() {
  const pool = await getSqlPool();

  const projectResult = await pool.request().query(`
    SELECT
      COUNT(*)                                                AS Total,
      SUM(CASE WHEN Status = 'Active'    THEN 1 ELSE 0 END)  AS Active,
      SUM(CASE WHEN Status = 'OnHold'    THEN 1 ELSE 0 END)  AS OnHold,
      SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END)  AS Completed,
      SUM(CASE WHEN Status = 'Cancelled' THEN 1 ELSE 0 END)  AS Cancelled
    FROM Projects
  `);

  const taskResult = await pool.request().query(`
    SELECT
      COUNT(*)                                              AS TotalTasks,
      SUM(CASE WHEN Status = 'Done' THEN 1 ELSE 0 END)     AS CompletedTasks
    FROM Tasks
  `);

  const p = projectResult.recordset[0];
  const t = taskResult.recordset[0];

  return {
    total:          p.Total,
    active:         p.Active,
    onHold:         p.OnHold,
    completed:      p.Completed,
    cancelled:      p.Cancelled,
    totalTasks:     t.TotalTasks,
    completedTasks: t.CompletedTasks,
  };
}

// ── Leave Stats from MongoDB ──────────────────────────────
async function getLeaveStats() {
  const db = await getMongoDb();

  const results = await db
    .collection('LeaveRequests')
    .aggregate([
      {
        $group: {
          _id:   '$status',
          count: { $sum: 1 },
        },
      },
    ])
    .toArray();

  const stats = {
    totalRequests: 0,
    pending:       0,
    approved:      0,
    rejected:      0,
    cancelled:     0,
  };

  results.forEach(r => {
    stats.totalRequests += r.count;
    switch (r._id) {
      case 'Pending':   stats.pending   = r.count; break;
      case 'Approved':  stats.approved  = r.count; break;
      case 'Rejected':  stats.rejected  = r.count; break;
      case 'Cancelled': stats.cancelled = r.count; break;
    }
  });

  return stats;
}

// ── Recent Activity from MongoDB ──────────────────────────
async function getRecentActivity(limit = 10) {
  const db = await getMongoDb();

  const logs = await db
    .collection('AuditLogs')
    .find({})
    .sort({ occurredAt: -1 })
    .limit(limit)
    .toArray();

  return logs.map(log => ({
    eventType:     log.eventType  || '',
    aggregateType: log.entityType || '',
    aggregateId:   parseInt(log.entityId) || 0,
    description:   log.eventType  || '',
    occurredAt:    log.occurredAt || new Date(),
  }));
}

// ── Build Full Report ─────────────────────────────────────
async function buildDashboardReport() {
  logger.info('Building dashboard report...');

  const [headcount, projects, leave, recentActivity] = await Promise.all([
    getHeadcountStats(),
    getProjectStats(),
    getLeaveStats(),
    getRecentActivity(),
  ]);

  const report = {
    reportKey:      'dashboard',
    generatedAt:    new Date(),
    headcount,
    projects,
    leave,
    recentActivity,
  };

  logger.info({
    msg:              'Dashboard report built',
    totalEmployees:   headcount.total,
    totalProjects:    projects.total,
    totalLeave:       leave.totalRequests,
    recentActivities: recentActivity.length,
  });

  return report;
}

module.exports = { buildDashboardReport };