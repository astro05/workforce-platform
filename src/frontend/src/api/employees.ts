import client from './client'
import type {
  Employee,
  CreateEmployeeDto,
  UpdateEmployeeDto,
  PagedResult,
  EmployeeQueryParams,
} from '../types'

export const employeeApi = {
  getAll: (params: EmployeeQueryParams) =>
    client.get<PagedResult<Employee>>('/employees', { params })
      .then(r => r.data),

  getById: (id: number) =>
    client.get<Employee>(`/employees/${id}`)
      .then(r => r.data),

  create: (dto: CreateEmployeeDto) =>
    client.post<Employee>('/employees', dto)
      .then(r => r.data),

  update: (id: number, dto: UpdateEmployeeDto) =>
    client.put<Employee>(`/employees/${id}`, dto)
      .then(r => r.data),

  delete: (id: number) =>
    client.delete(`/employees/${id}`),
}