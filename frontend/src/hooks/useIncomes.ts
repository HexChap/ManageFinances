import { useState, useEffect, useCallback } from 'react'
import type { Income } from '../types'
import {
  getIncomes,
  createIncome,
  updateIncome,
  deleteIncome,
  type CreateIncomeRequest,
  type UpdateIncomeRequest,
} from '../services/incomeService'

export function useIncomes() {
  const [incomes, setIncomes] = useState<Income[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(() => {
    setLoading(true)
    setError(null)
    getIncomes()
      .then(setIncomes)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  const add = useCallback(async (request: CreateIncomeRequest) => {
    const created = await createIncome(request)
    setIncomes((prev) => [created, ...prev])
  }, [])

  const update = useCallback(async (id: number, request: UpdateIncomeRequest) => {
    const updated = await updateIncome(id, request)
    setIncomes((prev) => prev.map((i) => (i.id === id ? updated : i)))
  }, [])

  const remove = useCallback(async (id: number) => {
    await deleteIncome(id)
    setIncomes((prev) => prev.filter((i) => i.id !== id))
  }, [])

  return { incomes, loading, error, add, update, remove }
}
