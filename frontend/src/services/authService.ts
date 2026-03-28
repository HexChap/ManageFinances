import axios from 'axios'
import api from './api'

export interface AuthUser {
  email: string
  isEmailConfirmed: boolean
}

export async function register(email: string, password: string): Promise<void> {
  await api.post('/register', { email, password })
}

export async function login(email: string, password: string): Promise<void> {
  await api.post('/login', { email, password }, { params: { useCookies: true } })
}

export async function logout(): Promise<void> {
  await api.post('/account/logout')
}

export async function getMe(): Promise<AuthUser | null> {
  try {
    const { data } = await api.get<AuthUser>('/manage/info')
    return data
  } catch (e) {
    if (axios.isAxiosError(e) && e.response?.status === 401) return null
    throw e
  }
}
