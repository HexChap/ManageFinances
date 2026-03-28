export interface User {
  id: number
  name: string
  email: string
}

export interface Category {
  id: number
  name: string
  userId: number | null
  createdAt: string
}

export interface Tag {
  id: number
  name: string
  userId: number
}

export interface Expense {
  id: number
  categoryId: number
  value: number
  userId: number
  createdAt: string
  tagIds: number[]
}

export interface Income {
  id: number
  value: number
  userId: number
  createdAt: string
}
