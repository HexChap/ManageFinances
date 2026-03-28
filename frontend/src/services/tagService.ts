import api from './api'
import type { Tag } from '../types'

export interface CreateTagRequest {
  name: string
}

export async function getTags(): Promise<Tag[]> {
  const { data } = await api.get<Tag[]>('/tags')
  return data
}

export async function createTag(request: CreateTagRequest): Promise<Tag> {
  const { data } = await api.post<Tag>('/tags', request)
  return data
}

export async function deleteTag(id: number): Promise<void> {
  await api.delete(`/tags/${id}`)
}
