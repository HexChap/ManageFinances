import { useState, useEffect, useCallback } from 'react'
import type { Income } from '../types'
import {
  getIncomes,
  createIncome,
  deleteIncome,
  type CreateIncomeRequest,
} from '../services/incomeService'

export function useIncomes(userId: number) {
  const [incomes, setIncomes] = useState<Income[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(() => {
    setLoading(true)
    setError(null)
    getIncomes(userId)
      .then(setIncomes)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false))
  }, [userId])

  useEffect(() => { load() }, [load])

  const add = useCallback(async (request: CreateIncomeRequest) => {
    const created = await createIncome(request)
    setIncomes((prev) => [created, ...prev])
  }, [])

  const remove = useCallback(async (id: number) => {
    await deleteIncome(id)
    setIncomes((prev) => prev.filter((i) => i.id !== id))
  }, [])

  return { incomes, loading, error, add, remove }
}
