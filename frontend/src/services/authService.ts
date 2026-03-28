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
  await api.post('/logout', {})
}

export async function getMe(): Promise<AuthUser> {
  const { data } = await api.get<AuthUser>('/manage/info')
  return data
}
