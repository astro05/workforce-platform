import client from './client'
import type { DashboardReport } from '../types'

export const dashboardApi = {
  getReport: () =>
    client.get<DashboardReport>('/dashboard')
      .then(r => r.data),
}