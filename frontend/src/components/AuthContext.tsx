import { createContext, useEffect, useState } from 'react'
import {
  login as authLogin,
  logout as authLogout,
  register as authRegister,
  getMe,
  type AuthUser,
} from '../services/authService'

export interface AuthContextValue {
  user: AuthUser | null
  loading: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => Promise<void>
  register: (email: string, password: string) => Promise<void>
}

export const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    getMe()
      .then(setUser)
      .catch(() => setUser(null))
      .finally(() => setLoading(false))
  }, [])

  const login = async (email: string, password: string) => {
    await authLogin(email, password)
    const me = await getMe()
    setUser(me)
  }

  const logout = async () => {
    await authLogout()
    setUser(null)
  }

  const register = async (email: string, password: string) => {
    await authRegister(email, password)
  }

  return (
    <AuthContext.Provider value={{ user, loading, login, logout, register }}>
      {children}
    </AuthContext.Provider>
  )
}
