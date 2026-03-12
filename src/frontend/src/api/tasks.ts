import client from './client'
import type { Task, CreateTaskDto, UpdateTaskDto } from '../types'

export const taskApi = {
  getByProject: (projectId: number) =>
    client.get<Task[]>('/tasks', { params: { projectId } })
      .then(r => r.data),

  getById: (id: number) =>
    client.get<Task>(`/tasks/${id}`)
      .then(r => r.data),

  create: (dto: CreateTaskDto) =>
    client.post<Task>('/tasks', dto)
      .then(r => r.data),

  update: (id: number, dto: UpdateTaskDto) =>
    client.put<Task>(`/tasks/${id}`, dto)
      .then(r => r.data),

  updateStatus: (id: number, status: string) =>
    client.patch<Task>(`/tasks/${id}/status`, { status })
      .then(r => r.data),

  delete: (id: number) =>
    client.delete(`/tasks/${id}`),
}