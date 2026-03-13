# Known Issues & Limitations

This document describes known issues, incomplete features,
and what would be addressed given more time.

---

## Current Known Issues

### 1. Dashboard Shows Empty on First Run

**Issue:** The dashboard page shows "report not yet available"
on first load after starting the system.

**Cause:** The Report Worker generates the dashboard report
every 5 minutes. On first startup, no report exists in MongoDB
until the first cron job completes.

**Workaround:** Wait up to 5 minutes after starting the Report
Worker, or restart it — it runs immediately on startup.

**Fix:** Add a loading state with a countdown timer on the
dashboard, or reduce the initial delay to run immediately.

---

### 2. Audit Logs Empty Until Events Are Published

**Issue:** The Audit Log page shows empty until domain events
are published by the API and consumed by the Audit Worker.

**Cause:** Audit logs are only written when the Audit Worker
receives events from RabbitMQ. No events are published until
a user performs an action (create/update/delete employee,
project, or task).

**Workaround:** Create, update, or delete any entity via the
frontend or Swagger. Events will be published and the Audit
Worker will write them to MongoDB within seconds.

---

### 3. RabbitMQ Credentials Must Match After Volume Wipe

**Issue:** After running `docker compose down -v`, RabbitMQ
reinitializes with credentials from `docker-compose.yml`. If
the credentials differ from what's in `appsettings.json`,
the workers fail to connect with `ACCESS_REFUSED`.

**Workaround:** Ensure `docker-compose.yml` and all
`appsettings.json` files use the same RabbitMQ credentials
(`admin/admin123`). If still failing, run:
```bash
docker exec -it workforce-platform-rabbitmq-1 \
  rabbitmqctl change_password admin admin123
```

---

### 4. Edit Employee Page Not Linked from Detail Page

**Issue:** The Edit button on the Employee Detail page
navigates to `/employees/:id/edit` which exists but there
is no back navigation from the list page to the edit page
directly. Minor UX gap.

**Fix:** Add an Edit button directly on the Employee List
table row actions.

---

### 5. No Pagination on Projects, Leave Requests, Audit Logs

**Issue:** Projects, Leave Requests, and Audit Logs load all
records at once without server-side pagination.

**Impact:** With large datasets this will cause slow load
times and high memory usage.

**Fix:** Implement server-side pagination for these endpoints
following the same `PagedResult<T>` pattern used for employees.

---

### 6. Task Status Update Has No Optimistic UI

**Issue:** When dragging a task to a new status column on the
Project Detail page, there is a brief flash as the page
re-fetches after the status update.

**Fix:** Implement optimistic updates using TanStack Query's
`useMutation` `onMutate` callback to update the cache
immediately before the server confirms.

---

## Incomplete Features

### 1. No Automated Tests

**What's Missing:** Unit tests for service layer, integration
tests for repositories, E2E tests for frontend flows.

**What Would Be Added:**
- xUnit tests for `EmployeeService`, `ProjectService`,
  `LeaveRequestService` with mocked repositories
- Integration tests using Testcontainers for SQL Server
  and MongoDB
- Playwright E2E tests for critical user flows:
  create employee, create project, submit leave request

---

### 2. No Real-Time Notifications

**What's Missing:** When a leave request is approved or
rejected, the employee has no real-time notification.

**What Would Be Added:** SignalR hub on the API server,
subscribed to by the frontend, publishing notifications
when leave status changes. The Audit Worker could trigger
these via a secondary RabbitMQ queue.

---

### 3. Task Events Not Published

**What's Missing:** `TaskService` does not currently publish
`TaskCreatedEvent` or `TaskStatusChangedEvent` to RabbitMQ.
Only `EmployeeService` and `ProjectService` publish events.

**Fix:** Wire `IEventPublisher` into `TaskService` and publish
events on create and status change, following the same pattern
as `EmployeeService`.

---

### 4. Leave Events Not Published

**What's Missing:** `LeaveRequestService` does not publish
`LeaveRequestCreatedEvent` or `LeaveRequestStatusChangedEvent`
to RabbitMQ.

**Fix:** Wire `IEventPublisher` into `LeaveRequestService`
and publish events on create and status change.

---

### 5. No Full-Text Search

**What's Missing:** Employee search only filters by name/email
prefix. No full-text search across projects, tasks, or leave
requests.

**Fix:** Add SQL Server CONTAINS full-text search for employees
and projects. Add MongoDB text indexes for leave request
reason field.

---

### 6. Authentication & Authorization
**What's Missing:** JWT authentication is not implemented—all API endpoints are currently publicly accessible to simplify 
local development and platform evaluation.

**Fix:** Integrate ASP.NET Core's built-in JWT middleware, which would seamlessly connect with the existing architecture. 
The platform already includes an actor field in all audit logs, ready to capture authenticated user identities with minimal code changes.

---

### 6. Testing
**What's Missing:** Automated tests were not implemented due to time constraints—the codebase currently lacks unit, integration, and end-to-end tests.

**Fix:** Implement a comprehensive testing strategy using xUnit for service layer unit tests 
(EmployeeService, ProjectService, LeaveService), TestContainers for integration tests against 
real PostgreSQL and MongoDB instances, and Playwright for end-to-end tests that validate critical 
user flows against the fully running Docker stack.

---

