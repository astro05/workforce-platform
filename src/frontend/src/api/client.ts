import axios from 'axios'
import { message } from 'antd'

const client = axios.create({
  baseURL: '/api/v1',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30_000,
})

// Response interceptor — global error handling
client.interceptors.response.use(
  response => response,
  error => {
    const msg =
      error.response?.data?.message ||
      error.message ||
      'An unexpected error occurred'

    message.error(msg)
    return Promise.reject(error)
  }
)

export default client