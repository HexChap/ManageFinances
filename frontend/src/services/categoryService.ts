import api from './api'
import type { Category } from '../types'

export interface CreateCategoryRequest {
  name: string
  userId?: number | null
}

export async function getCategories(userId: number): Promise<Category[]> {
  const { data } = await api.get<Category[]>('/categories', { params: { userId } })
  return data
}

export async function createCategory(request: CreateCategoryRequest): Promise<Category> {
  const { data } = await api.post<Category>('/categories', request)
  return data
}

export async function deleteCategory(id: number): Promise<void> {
  await api.delete(`/categories/${id}`)
}
