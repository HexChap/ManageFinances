import api from './api'
import type { Category } from '../types'

export interface CreateCategoryRequest {
  name: string
}

export async function getCategories(): Promise<Category[]> {
  const { data } = await api.get<Category[]>('/categories')
  return data
}

export async function createCategory(request: CreateCategoryRequest): Promise<Category> {
  const { data } = await api.post<Category>('/categories', request)
  return data
}

export interface UpdateCategoryRequest {
  name: string
}

export async function updateCategory(id: number, request: UpdateCategoryRequest): Promise<Category> {
  const { data } = await api.put<Category>(`/categories/${id}`, request)
  return data
}

export async function deleteCategory(id: number): Promise<void> {
  await api.delete(`/categories/${id}`)
}
