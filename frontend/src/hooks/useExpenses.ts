import { useState, useEffect, useCallback } from 'react'
import type { Expense } from '../types'
import {
  getExpenses,
  createExpense,
  updateExpense,
  deleteExpense,
  type ExpensePeriod,
  type CreateExpenseRequest,
  type UpdateExpenseRequest,
} from '../services/expenseService'

export function useExpenses(period: ExpensePeriod = 'All') {
  const [expenses, setExpenses] = useState<Expense[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(() => {
    setLoading(true)
    setError(null)
    getExpenses(period)
      .then(setExpenses)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false))
  }, [period])

  useEffect(() => { load() }, [load])

  const add = useCallback(async (request: CreateExpenseRequest) => {
    const created = await createExpense(request)
    setExpenses((prev) => [created, ...prev])
  }, [])

  const update = useCallback(async (id: number, request: UpdateExpenseRequest) => {
    const updated = await updateExpense(id, request)
    setExpenses((prev) => prev.map((e) => (e.id === id ? updated : e)))
  }, [])

  const remove = useCallback(async (id: number) => {
    await deleteExpense(id)
    setExpenses((prev) => prev.filter((e) => e.id !== id))
  }, [])

  return { expenses, loading, error, add, update, remove }
}
