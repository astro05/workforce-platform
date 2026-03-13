# Workforce Management Platform

![CI](https://github.com/astro05/workforce-platform/actions/workflows/ci.yml/badge.svg)

A full-stack distributed Workforce Management Platform built with
.NET 10, React TypeScript, MongoDB, SQL Server, RabbitMQ and Docker.

## Tech Stack

| Layer | Technology |
|---|---|
| API | .NET 10, ASP.NET Core, EF Core |
| Frontend | React 18, TypeScript, Vite, Ant Design |
| SQL Database | SQL Server 2022 |
| Document Database | MongoDB 7 |
| Message Broker | RabbitMQ 3 |
| Audit Worker | .NET 10 Background Worker |
| Report Worker | Node.js 20 |
| Container | Docker + Docker Compose |
| CI/CD | GitHub Actions |

## Architecture
```
React Frontend
      ↓
.NET 10 API (REST)
   ↓         ↓
SQL Server  MongoDB
      ↓
  RabbitMQ
   ↓      ↓
Audit    Report
Worker   Worker
(.NET)  (Node.js)
```

## Quick Start

### Prerequisites
- Docker Desktop
- .NET 10 SDK
- Node.js 20+

### Run with Docker Compose
```bash
docker compose up --build
```

### Run Locally (Development)

**Step 1 — Start infrastructure:**
```bash
docker compose up sqlserver mongo rabbitmq -d
```

**Step 2 — Run API (Terminal 1):**
```bash
cd src/api/WorkforceAPI
dotnet run
```

**Step 3 — Run Audit Worker (Terminal 2):**
```bash
cd src/workers/audit-worker/AuditWorker
dotnet run
```

**Step 4 — Run Report Worker (Terminal 3):**
```bash
cd src/workers/report-worker
node src/index.js
```

**Step 5 — Run Frontend (Terminal 4):**
```bash
cd src/frontend
npm run dev
```

## Service URLs

| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| API Swagger | http://localhost:5000/swagger |
| RabbitMQ UI | http://localhost:15672 |

## Environment Variables

| Variable | Default | Description |
|---|---|---|
| `SA_PASSWORD` | `Workforce_Pass123` | SQL Server password |
| `MONGO_USER` | `mongo_user` | MongoDB username |
| `MONGO_PASSWORD` | `mongo_pass` | MongoDB password |
| `RABBITMQ_USER` | `admin` | RabbitMQ username |
| `RABBITMQ_PASSWORD` | `admin123` | RabbitMQ password |

## Technology Choices

### SQL Server
Chosen for strong EF Core integration,
ACID compliance, and relational integrity
for employee/project/task domain data.

### MongoDB
Document-oriented storage for leave requests
(embedded approval history) and operational
data (audit logs, dashboard reports).

### RabbitMQ
Simple, reliable message broker with excellent
.NET and Node.js client support. Management UI
included for observability.

### Node.js for Report Worker
Scheduled I/O-bound aggregation is a natural
fit for Node's event loop. Distinct from the
reactive .NET audit worker.