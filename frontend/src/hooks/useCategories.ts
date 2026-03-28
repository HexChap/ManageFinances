import { useState, useEffect, useCallback } from 'react'
import type { Category } from '../types'
import {
  getCategories,
  createCategory,
  deleteCategory,
  type CreateCategoryRequest,
} from '../services/categoryService'

export function useCategories(userId: number) {
  const [categories, setCategories] = useState<Category[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(() => {
    setLoading(true)
    setError(null)
    getCategories(userId)
      .then(setCategories)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false))
  }, [userId])

  useEffect(() => { load() }, [load])

  const add = useCallback(async (request: CreateCategoryRequest) => {
    const created = await createCategory(request)
    setCategories((prev) => [...prev, created])
  }, [])

  const remove = useCallback(async (id: number) => {
    await deleteCategory(id)
    setCategories((prev) => prev.filter((c) => c.id !== id))
  }, [])

  return { categories, loading, error, add, remove }
}
