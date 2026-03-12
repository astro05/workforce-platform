'use strict';

const sql          = require('mssql');
const { MongoClient } = require('mongodb');
const logger       = require('./logger');

// ── SQL Server ────────────────────────────────────────────
let sqlPool = null;

async function getSqlPool() {
  if (sqlPool) return sqlPool;

  const config = {
    server:   process.env.MSSQL_HOST     || 'localhost',
    port:     parseInt(process.env.MSSQL_PORT || '1433'),
    database: process.env.MSSQL_DATABASE || 'WorkforceDb',
    user:     process.env.MSSQL_USER     || 'sa',
    password: process.env.MSSQL_PASSWORD || 'Workforce_Pass123',
    options: {
      trustServerCertificate: true,
      enableArithAbort:       true,
      connectTimeout:         30000,
    },
  };

  logger.info('Connecting to SQL Server...');
  sqlPool = await sql.connect(config);
  logger.info('SQL Server connected');
  return sqlPool;
}

// ── MongoDB ───────────────────────────────────────────────
let mongoClient = null;
let mongoDb     = null;

async function getMongoDb() {
  if (mongoDb) return mongoDb;

  const mongoUrl  = process.env.MONGO_URL     || 'mongodb://mongo_user:mongo_pass@localhost:27017/?authSource=admin';
  const mongoName = process.env.MONGO_DB_NAME || 'WorkforceDb';

  logger.info('Connecting to MongoDB...');
  mongoClient = new MongoClient(mongoUrl);
  await mongoClient.connect();
  mongoDb = mongoClient.db(mongoName);
  logger.info('MongoDB connected');
  return mongoDb;
}

// ── Graceful shutdown ─────────────────────────────────────
async function closeConnections() {
  if (sqlPool)     { await sqlPool.close();     sqlPool     = null; }
  if (mongoClient) { await mongoClient.close(); mongoClient = null; mongoDb = null; }
  logger.info('Database connections closed');
}

module.exports = { getSqlPool, getMongoDb, closeConnections };