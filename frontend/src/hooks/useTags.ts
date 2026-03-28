import { useState, useEffect, useCallback } from 'react'
import type { Tag } from '../types'
import { getTags, createTag, deleteTag, type CreateTagRequest } from '../services/tagService'

export function useTags(userId: number) {
  const [tags, setTags] = useState<Tag[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(() => {
    setLoading(true)
    setError(null)
    getTags(userId)
      .then(setTags)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false))
  }, [userId])

  useEffect(() => { load() }, [load])

  const add = useCallback(async (request: CreateTagRequest) => {
    const created = await createTag(request)
    setTags((prev) => [...prev, created])
  }, [])

  const remove = useCallback(async (id: number) => {
    await deleteTag(id)
    setTags((prev) => prev.filter((t) => t.id !== id))
  }, [])

  return { tags, loading, error, add, remove }
}
