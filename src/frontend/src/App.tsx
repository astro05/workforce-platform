import { Routes, Route} from 'react-router-dom'
import AppLayout          from './components/Layout/AppLayout'
import DashboardPage      from './pages/dashboard/DashboardPage'
import EmployeeListPage   from './pages/employees/EmployeeListPage'
import EmployeeDetailPage from './pages/employees/EmployeeDetailPage'
import CreateEmployeePage from './pages/employees/CreateEmployeePage'
import EditEmployeePage   from './pages/employees/EditEmployeePage'
import ProjectListPage    from './pages/projects/ProjectListPage'
import ProjectDetailPage  from './pages/projects/ProjectDetailPage'
import LeaveListPage      from './pages/leave/LeaveListPage'
import CreateLeavePage    from './pages/leave/CreateLeavePage'
import AuditLogPage       from './pages/audit/AuditLogPage'
import NotFoundPage       from './pages/NotFoundPage'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<AppLayout />}>
        <Route index element={<DashboardPage />} />

        {/* Employees */}
        <Route path="employees"
          element={<EmployeeListPage />} />
        <Route path="employees/new"
          element={<CreateEmployeePage />} />
        <Route path="employees/:id"
          element={<EmployeeDetailPage />} />
        <Route path="employees/:id/edit"
          element={<EditEmployeePage />} />

        {/* Projects */}
        <Route path="projects"
          element={<ProjectListPage />} />
        <Route path="projects/:id"
          element={<ProjectDetailPage />} />

        {/* Leave */}
        <Route path="leave"
          element={<LeaveListPage />} />
        <Route path="leave/new"
          element={<CreateLeavePage />} />

        {/* Audit */}
        <Route path="audit"
          element={<AuditLogPage />} />

        {/* 404 */}
        <Route path="*"
          element={<NotFoundPage />} />
      </Route>
    </Routes>
  )
}