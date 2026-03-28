import { useState, useEffect, useCallback } from 'react'
import type { Expense } from '../types'
import {
  getExpenses,
  createExpense,
  deleteExpense,
  type ExpensePeriod,
  type CreateExpenseRequest,
} from '../services/expenseService'

export function useExpenses(userId: number, period: ExpensePeriod = 'All') {
  const [expenses, setExpenses] = useState<Expense[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(() => {
    setLoading(true)
    setError(null)
    getExpenses(userId, period)
      .then(setExpenses)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false))
  }, [userId, period])

  useEffect(() => { load() }, [load])

  const add = useCallback(async (request: CreateExpenseRequest) => {
    const created = await createExpense(request)
    setExpenses((prev) => [created, ...prev])
  }, [])

  const remove = useCallback(async (id: number) => {
    await deleteExpense(id)
    setExpenses((prev) => prev.filter((e) => e.id !== id))
  }, [])

  return { expenses, loading, error, add, remove }
}
