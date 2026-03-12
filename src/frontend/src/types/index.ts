// ── Common ────────────────────────────────────────────────
export interface PagedResult<T> {
  items:      T[]
  totalCount: number
  page:       number
  pageSize:   number
  totalPages: number
}

export interface EmployeeQueryParams {
  page?:         number
  pageSize?:     number
  search?:       string
  departmentId?: number
  isActive?:     boolean
}

// ── Department & Designation ──────────────────────────────
export interface Department {
  id:            number
  name:          string
  description?:  string
  employeeCount: number
  createdAt:     string
}

export interface Designation {
  id:            number
  name:          string
  level?:        string
  employeeCount: number
  createdAt:     string
}

// ── Employee ──────────────────────────────────────────────
export interface Employee {
  id:              number
  firstName:       string
  lastName:        string
  fullName:        string
  email:           string
  isActive:        boolean
  salary:          number
  joiningDate:     string
  phone?:          string
  address?:        string
  city?:           string
  country?:        string
  avatarUrl?:      string
  skills:          string[]
  departmentId:    number
  departmentName:  string
  designationId:   number
  designationName: string
  createdAt:       string
  updatedAt:       string
}

export interface CreateEmployeeDto {
  firstName:    string
  lastName:     string
  email:        string
  salary:       number
  joiningDate:  string
  departmentId: number
  designationId:number
  phone?:       string
  address?:     string
  city?:        string
  country?:     string
  avatarUrl?:   string
  skills:       string[]
}

export interface UpdateEmployeeDto extends CreateEmployeeDto {
  isActive: boolean
}

// ── Project ───────────────────────────────────────────────
export interface ProjectMember {
  employeeId:     number
  fullName:       string
  email:          string
  role:           string
  avatarUrl?:     string
  departmentName: string
}

export interface Project {
  id:                 number
  name:               string
  description?:       string
  status:             string
  startDate:          string
  endDate?:           string
  memberCount:        number
  taskCount:          number
  completedTaskCount: number
  members:            ProjectMember[]
  tasks:              Task[]
  createdAt:          string
  updatedAt:          string
}

export interface CreateProjectDto {
  name:        string
  description?: string
  status:      string
  startDate:   string
  endDate?:    string
  memberIds:   number[]
}

export interface UpdateProjectDto {
  name:        string
  description?: string
  status:      string
  startDate:   string
  endDate?:    string
}

export interface AddMemberDto {
  employeeId: number
  role:       string
}

// ── Task ──────────────────────────────────────────────────
export interface Task {
  id:             number
  title:          string
  description?:   string
  status:         string
  priority:       string
  dueDate?:       string
  projectId:      number
  assignedToId?:  number
  assignedToName?:string
  createdAt:      string
  updatedAt:      string
}

export interface CreateTaskDto {
  title:        string
  description?: string
  status:       string
  priority:     string
  dueDate?:     string
  projectId:    number
  assignedToId?:number
}

export interface UpdateTaskDto {
  title:        string
  description?: string
  status:       string
  priority:     string
  dueDate?:     string
  assignedToId?:number
}

// ── Leave Request ─────────────────────────────────────────
export interface ApprovalEntry {
  status:    string
  comment?:  string
  actorName: string
  changedAt: string
}

export interface LeaveRequest {
  id:              string
  employeeId:      number
  employeeName:    string
  leaveType:       string
  startDate:       string
  endDate:         string
  totalDays:       number
  status:          string
  reason?:         string
  approvalHistory: ApprovalEntry[]
  createdAt:       string
  updatedAt:       string
}

export interface CreateLeaveRequestDto {
  employeeId:   number
  employeeName: string
  leaveType:    string
  startDate:    string
  endDate:      string
  reason?:      string
}

export interface UpdateLeaveStatusDto {
  status:    string
  comment?:  string
  actorName: string
}

export interface LeaveQueryParams {
  status?:    string
  leaveType?: string
  employeeId?:number
}

// ── Dashboard ─────────────────────────────────────────────
export interface DepartmentHeadcount {
  department: string
  count:      number
}

export interface HeadcountStats {
  total:        number
  active:       number
  inactive:     number
  byDepartment: DepartmentHeadcount[]
}

export interface ProjectStats {
  total:          number
  active:         number
  onHold:         number
  completed:      number
  cancelled:      number
  totalTasks:     number
  completedTasks: number
}

export interface LeaveStats {
  totalRequests: number
  pending:       number
  approved:      number
  rejected:      number
  cancelled:     number
}

export interface RecentActivity {
  eventType:     string
  aggregateType: string
  aggregateId:   number
  description:   string
  occurredAt:    string
}

export interface DashboardReport {
  generatedAt:    string
  headcount:      HeadcountStats
  projects:       ProjectStats
  leave:          LeaveStats
  recentActivity: RecentActivity[]
}

// ── Audit Log ─────────────────────────────────────────────
export interface AuditLog {
  id:            string
  eventType:     string
  aggregateType: string
  aggregateId:   number
  actorName?:    string
  description:   string
  before?:       string
  after?:        string
  occurredAt:    string
}