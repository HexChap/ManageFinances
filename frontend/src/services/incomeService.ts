import api from './api'
import type { Income } from '../types'

export interface CreateIncomeRequest {
  value: number
  userId: number
}

export async function getIncomes(userId: number): Promise<Income[]> {
  const { data } = await api.get<Income[]>('/incomes', { params: { userId } })
  return data
}

export async function createIncome(request: CreateIncomeRequest): Promise<Income> {
  const { data } = await api.post<Income>('/incomes', request)
  return data
}

export async function deleteIncome(id: number): Promise<void> {
  await api.delete(`/incomes/${id}`)
}
