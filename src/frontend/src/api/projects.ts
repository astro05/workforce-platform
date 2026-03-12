import client from './client'
import type {
  Project,
  CreateProjectDto,
  UpdateProjectDto,
  AddMemberDto,
} from '../types'

export const projectApi = {
  getAll: () =>
    client.get<Project[]>('/projects')
      .then(r => r.data),

  getById: (id: number) =>
    client.get<Project>(`/projects/${id}`)
      .then(r => r.data),

  create: (dto: CreateProjectDto) =>
    client.post<Project>('/projects', dto)
      .then(r => r.data),

  update: (id: number, dto: UpdateProjectDto) =>
    client.put<Project>(`/projects/${id}`, dto)
      .then(r => r.data),

  addMember: (id: number, dto: AddMemberDto) =>
    client.post(`/projects/${id}/members`, dto),

  removeMember: (id: number, employeeId: number) =>
    client.delete(`/projects/${id}/members/${employeeId}`),
}