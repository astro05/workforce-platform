import client from './client'
import type {
  LeaveRequest,
  CreateLeaveRequestDto,
  UpdateLeaveStatusDto,
  LeaveQueryParams,
} from '../types'

export const leaveApi = {
  getAll: (params?: LeaveQueryParams) =>
    client.get<LeaveRequest[]>('/leaverequests', { params })
      .then(r => r.data),

  getByEmployee: (employeeId: number) =>
    client.get<LeaveRequest[]>(
      `/leaverequests/employee/${employeeId}`)
      .then(r => r.data),

  getById: (id: string) =>
    client.get<LeaveRequest>(`/leaverequests/${id}`)
      .then(r => r.data),

  create: (dto: CreateLeaveRequestDto) =>
    client.post<LeaveRequest>('/leaverequests', dto)
      .then(r => r.data),

  updateStatus: (id: string, dto: UpdateLeaveStatusDto) =>
    client.put<LeaveRequest>(
      `/leaverequests/${id}/status`, dto)
      .then(r => r.data),

  cancel: (id: string, actorName: string) =>
    client.put(`/leaverequests/${id}/cancel`,
      null, { params: { actorName } }),
}