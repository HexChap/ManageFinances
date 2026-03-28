import { useState, useEffect, useCallback } from 'react'
import type { Category } from '../types'
import {
  getCategories,
  createCategory,
  updateCategory,
  deleteCategory,
  type CreateCategoryRequest,
  type UpdateCategoryRequest,
} from '../services/categoryService'

export function useCategories() {
  const [categories, setCategories] = useState<Category[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(() => {
    setLoading(true)
    setError(null)
    getCategories()
      .then(setCategories)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  const add = useCallback(async (request: CreateCategoryRequest) => {
    const created = await createCategory(request)
    setCategories((prev) => [...prev, created])
  }, [])

  const update = useCallback(async (id: number, request: UpdateCategoryRequest) => {
    const updated = await updateCategory(id, request)
    setCategories((prev) => prev.map((c) => (c.id === id ? updated : c)))
  }, [])

  const remove = useCallback(async (id: number) => {
    await deleteCategory(id)
    setCategories((prev) => prev.filter((c) => c.id !== id))
  }, [])

  return { categories, loading, error, add, update, remove }
}
