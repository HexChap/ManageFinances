import api from './api'
import type { Expense } from '../types'

export type ExpensePeriod = 'All' | 'Today' | 'Month'

export interface CreateExpenseRequest {
  categoryId: number
  value: number
  userId: number
  tagIds?: number[]
}

export async function getExpenses(userId: number, period: ExpensePeriod = 'All'): Promise<Expense[]> {
  const { data } = await api.get<Expense[]>('/expenses', { params: { userId, period } })
  return data
}

export async function createExpense(request: CreateExpenseRequest): Promise<Expense> {
  const { data } = await api.post<Expense>('/expenses', request)
  return data
}

export async function deleteExpense(id: number): Promise<void> {
  await api.delete(`/expenses/${id}`)
}
