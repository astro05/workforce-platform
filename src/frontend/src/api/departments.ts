import client from './client'
import type { Department, Designation } from '../types'

export const departmentApi = {
  getAll: () =>
    client.get<Department[]>('/departments')
      .then(r => r.data),
}

export const designationApi = {
  getAll: () =>
    client.get<Designation[]>('/designations')
      .then(r => r.data),
}