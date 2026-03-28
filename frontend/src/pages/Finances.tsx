import { useState } from "react"
import {
  PlusIcon,
  Trash2Icon,
  TagIcon,
  FolderIcon,
  TrendingUpIcon,
  TrendingDownIcon,
  XIcon,
} from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogFooter,
} from "@/components/ui/dialog"
import { Label } from "@/components/ui/label"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"

type Tag = { id: number; name: string; userId: number }
type Category = { id: number; name: string; userId: number }
type Expense = {
  id: number
  value: number
  userId: number
  categoryId: number
  tagIds: number[]
}
type Income = { id: number; value: number; userId: number }

const MOCK_USER_ID = 1

const initialCategories: Category[] = [
  { id: 1, name: "Храна и напитки", userId: 1 },
  { id: 2, name: "Транспорт", userId: 1 },
  { id: 3, name: "Комунални услуги", userId: 1 },
]
const initialTags: Tag[] = [
  { id: 1, name: "основно", userId: 1 },
  { id: 2, name: "повтарящо се", userId: 1 },
  { id: 3, name: "еднократно", userId: 1 },
]
const initialExpenses: Expense[] = [
  { id: 1, value: 42.5, userId: 1, categoryId: 1, tagIds: [1, 2] },
  { id: 2, value: 15.0, userId: 1, categoryId: 2, tagIds: [1] },
]
const initialIncomes: Income[] = [
  { id: 1, value: 3200.0, userId: 1 },
  { id: 2, value: 500.0, userId: 1 },
]

export const Finances = () => {
  const [categories, setCategories] = useState<Category[]>(initialCategories)
  const [tags, setTags] = useState<Tag[]>(initialTags)
  const [expenses, setExpenses] = useState<Expense[]>(initialExpenses)
  const [incomes, setIncomes] = useState<Income[]>(initialIncomes)

  const [nextId, setNextId] = useState(100)
  const genId = () => {
    setNextId((n) => n + 1)
    return nextId
  }

  const [catName, setCatName] = useState("")
  const [catOpen, setCatOpen] = useState(false)

  const [tagName, setTagName] = useState("")
  const [tagOpen, setTagOpen] = useState(false)

  const [expValue, setExpValue] = useState("")
  const [expCatId, setExpCatId] = useState("")
  const [expTagIds, setExpTagIds] = useState<number[]>([])
  const [expOpen, setExpOpen] = useState(false)

  const [incValue, setIncValue] = useState("")
  const [incOpen, setIncOpen] = useState(false)

  const addCategory = () => {
    if (!catName.trim()) return
    setCategories((c) => [
      ...c,
      { id: genId(), name: catName.trim(), userId: MOCK_USER_ID },
    ])
    setCatName("")
    setCatOpen(false)
  }

  const addTag = () => {
    if (!tagName.trim()) return
    setTags((t) => [
      ...t,
      { id: genId(), name: tagName.trim(), userId: MOCK_USER_ID },
    ])
    setTagName("")
    setTagOpen(false)
  }

  const addExpense = () => {
    const val = parseFloat(expValue)
    if (isNaN(val) || !expCatId) return
    setExpenses((e) => [
      ...e,
      {
        id: genId(),
        value: val,
        userId: MOCK_USER_ID,
        categoryId: parseInt(expCatId),
        tagIds: expTagIds,
      },
    ])
    setExpValue("")
    setExpCatId("")
    setExpTagIds([])
    setExpOpen(false)
  }

  const addIncome = () => {
    const val = parseFloat(incValue)
    if (isNaN(val)) return
    setIncomes((i) => [...i, { id: genId(), value: val, userId: MOCK_USER_ID }])
    setIncValue("")
    setIncOpen(false)
  }

  const deleteCategory = (id: number) => {
    setCategories((c) => c.filter((x) => x.id !== id))
    setExpenses((e) => e.filter((x) => x.categoryId !== id))
  }
  const deleteTag = (id: number) => {
    setTags((t) => t.filter((x) => x.id !== id))
    setExpenses((e) =>
      e.map((x) => ({ ...x, tagIds: x.tagIds.filter((tid) => tid !== id) }))
    )
  }
  const deleteExpense = (id: number) =>
    setExpenses((e) => e.filter((x) => x.id !== id))
  const deleteIncome = (id: number) =>
    setIncomes((i) => i.filter((x) => x.id !== id))

  const getCategoryName = (id: number) =>
    categories.find((c) => c.id === id)?.name ?? "—"
  const getTagName = (id: number) => tags.find((t) => t.id === id)?.name ?? ""

  const totalExpenses = expenses.reduce((s, e) => s + e.value, 0)
  const totalIncomes = incomes.reduce((s, i) => s + i.value, 0)
  const balance = totalIncomes - totalExpenses

  const toggleExpTag = (id: number) =>
    setExpTagIds((ids) =>
      ids.includes(id) ? ids.filter((x) => x !== id) : [...ids, id]
    )

  return (
    <div className="min-h-screen bg-background p-4 sm:p-6 lg:p-8">
      <div className="mx-auto max-w-5xl space-y-6">
        {/* Заглавие */}
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">
            Финансов мениджър
          </h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Потребител ID: {MOCK_USER_ID} · Часова зона: Europe/Sofia
          </p>
        </div>

        {/* Обобщение */}
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
                <TrendingUpIcon className="size-4 text-emerald-500" /> Общи
                приходи
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-2xl font-semibold text-emerald-600">
                +${totalIncomes.toFixed(2)}
              </p>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
                <TrendingDownIcon className="size-4 text-rose-500" /> Общи
                разходи
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-2xl font-semibold text-rose-600">
                -${totalExpenses.toFixed(2)}
              </p>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">
                Баланс
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p
                className={`text-2xl font-semibold ${balance >= 0 ? "text-emerald-600" : "text-rose-600"}`}
              >
                {balance >= 0 ? "+" : ""}${balance.toFixed(2)}
              </p>
            </CardContent>
          </Card>
        </div>

        {/* Основни табове */}
        <Tabs defaultValue="expenses">
          <TabsList className="grid w-full grid-cols-4">
            <TabsTrigger value="expenses">Разходи</TabsTrigger>
            <TabsTrigger value="incomes">Приходи</TabsTrigger>
            <TabsTrigger value="categories">Категории</TabsTrigger>
            <TabsTrigger value="tags">Тагове</TabsTrigger>
          </TabsList>

          {/* РАЗХОДИ */}
          <TabsContent value="expenses" className="mt-4">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between pb-4">
                <CardTitle className="text-base">Разходи</CardTitle>
                <Dialog open={expOpen} onOpenChange={setExpOpen}>
                  <DialogTrigger asChild>
                    <Button size="sm">
                      <PlusIcon className="mr-1 size-4" />
                      Добави разход
                    </Button>
                  </DialogTrigger>
                  <DialogContent>
                    <DialogHeader>
                      <DialogTitle>Нов разход</DialogTitle>
                    </DialogHeader>
                    <div className="space-y-4 py-2">
                      <div className="space-y-1.5">
                        <Label>Сума ($)</Label>
                        <Input
                          type="number"
                          placeholder="0.00"
                          value={expValue}
                          onChange={(e) => setExpValue(e.target.value)}
                        />
                      </div>
                      <div className="space-y-1.5">
                        <Label>Категория</Label>
                        <Select value={expCatId} onValueChange={setExpCatId}>
                          <SelectTrigger>
                            <SelectValue placeholder="Избери категория" />
                          </SelectTrigger>
                          <SelectContent>
                            {categories.map((c) => (
                              <SelectItem key={c.id} value={String(c.id)}>
                                {c.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                      <div className="space-y-1.5">
                        <Label>Тагове</Label>
                        <div className="flex flex-wrap gap-2">
                          {tags.map((t) => (
                            <Badge
                              key={t.id}
                              variant={
                                expTagIds.includes(t.id) ? "default" : "outline"
                              }
                              className="cursor-pointer select-none"
                              onClick={() => toggleExpTag(t.id)}
                            >
                              {t.name}
                            </Badge>
                          ))}
                        </div>
                      </div>
                    </div>
                    <DialogFooter>
                      <Button onClick={addExpense}>Запази</Button>
                    </DialogFooter>
                  </DialogContent>
                </Dialog>
              </CardHeader>
              <CardContent className="p-0">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead className="w-12">ID</TableHead>
                      <TableHead>Сума</TableHead>
                      <TableHead>Категория</TableHead>
                      <TableHead>Тагове</TableHead>
                      <TableHead className="w-12"></TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {expenses.length === 0 && (
                      <TableRow>
                        <TableCell
                          colSpan={5}
                          className="py-8 text-center text-muted-foreground"
                        >
                          Няма разходи
                        </TableCell>
                      </TableRow>
                    )}
                    {expenses.map((exp) => (
                      <TableRow key={exp.id}>
                        <TableCell className="text-xs text-muted-foreground">
                          {exp.id}
                        </TableCell>
                        <TableCell className="font-medium text-rose-600">
                          -${exp.value.toFixed(2)}
                        </TableCell>
                        <TableCell>
                          <Badge variant="secondary">
                            {getCategoryName(exp.categoryId)}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <div className="flex flex-wrap gap-1">
                            {exp.tagIds.map((tid) => (
                              <Badge
                                key={tid}
                                variant="outline"
                                className="text-xs"
                              >
                                {getTagName(tid)}
                              </Badge>
                            ))}
                          </div>
                        </TableCell>
                        <TableCell>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="size-8 text-muted-foreground hover:text-destructive"
                            onClick={() => deleteExpense(exp.id)}
                          >
                            <Trash2Icon className="size-3.5" />
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </TabsContent>

          {/* ПРИХОДИ */}
          <TabsContent value="incomes" className="mt-4">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between pb-4">
                <CardTitle className="text-base">Приходи</CardTitle>
                <Dialog open={incOpen} onOpenChange={setIncOpen}>
                  <DialogTrigger asChild>
                    <Button size="sm">
                      <PlusIcon className="mr-1 size-4" />
                      Добави приход
                    </Button>
                  </DialogTrigger>
                  <DialogContent>
                    <DialogHeader>
                      <DialogTitle>Нов приход</DialogTitle>
                    </DialogHeader>
                    <div className="space-y-4 py-2">
                      <div className="space-y-1.5">
                        <Label>Сума ($)</Label>
                        <Input
                          type="number"
                          placeholder="0.00"
                          value={incValue}
                          onChange={(e) => setIncValue(e.target.value)}
                        />
                      </div>
                    </div>
                    <DialogFooter>
                      <Button onClick={addIncome}>Запази</Button>
                    </DialogFooter>
                  </DialogContent>
                </Dialog>
              </CardHeader>
              <CardContent className="p-0">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead className="w-12">ID</TableHead>
                      <TableHead>Сума</TableHead>
                      <TableHead className="w-12"></TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {incomes.length === 0 && (
                      <TableRow>
                        <TableCell
                          colSpan={3}
                          className="py-8 text-center text-muted-foreground"
                        >
                          Няма приходи
                        </TableCell>
                      </TableRow>
                    )}
                    {incomes.map((inc) => (
                      <TableRow key={inc.id}>
                        <TableCell className="text-xs text-muted-foreground">
                          {inc.id}
                        </TableCell>
                        <TableCell className="font-medium text-emerald-600">
                          +${inc.value.toFixed(2)}
                        </TableCell>
                        <TableCell>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="size-8 text-muted-foreground hover:text-destructive"
                            onClick={() => deleteIncome(inc.id)}
                          >
                            <Trash2Icon className="size-3.5" />
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </TabsContent>

          {/* КАТЕГОРИИ */}
          <TabsContent value="categories" className="mt-4">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between pb-4">
                <CardTitle className="text-base">Категории</CardTitle>
                <Dialog open={catOpen} onOpenChange={setCatOpen}>
                  <DialogTrigger asChild>
                    <Button size="sm">
                      <PlusIcon className="mr-1 size-4" />
                      Добави категория
                    </Button>
                  </DialogTrigger>
                  <DialogContent>
                    <DialogHeader>
                      <DialogTitle>Нова категория</DialogTitle>
                    </DialogHeader>
                    <div className="space-y-4 py-2">
                      <div className="space-y-1.5">
                        <Label>Име</Label>
                        <Input
                          placeholder="напр. Хранителни стоки"
                          value={catName}
                          onChange={(e) => setCatName(e.target.value)}
                          onKeyDown={(e) => e.key === "Enter" && addCategory()}
                        />
                      </div>
                    </div>
                    <DialogFooter>
                      <Button onClick={addCategory}>Запази</Button>
                    </DialogFooter>
                  </DialogContent>
                </Dialog>
              </CardHeader>
              <CardContent>
                <div className="space-y-2">
                  {categories.length === 0 && (
                    <p className="py-8 text-center text-muted-foreground">
                      Няма категории
                    </p>
                  )}
                  {categories.map((cat) => {
                    const count = expenses.filter(
                      (e) => e.categoryId === cat.id
                    ).length
                    return (
                      <div
                        key={cat.id}
                        className="flex items-center justify-between rounded-lg border px-4 py-3"
                      >
                        <div className="flex items-center gap-3">
                          <FolderIcon className="size-4 text-muted-foreground" />
                          <span className="text-sm font-medium">
                            {cat.name}
                          </span>
                          <Badge variant="secondary" className="text-xs">
                            {count} {count === 1 ? "разход" : "разхода"}
                          </Badge>
                        </div>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="size-8 text-muted-foreground hover:text-destructive"
                          onClick={() => deleteCategory(cat.id)}
                        >
                          <Trash2Icon className="size-3.5" />
                        </Button>
                      </div>
                    )
                  })}
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          {/* ТАГОВЕ */}
          <TabsContent value="tags" className="mt-4">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between pb-4">
                <CardTitle className="text-base">Тагове</CardTitle>
                <Dialog open={tagOpen} onOpenChange={setTagOpen}>
                  <DialogTrigger asChild>
                    <Button size="sm">
                      <PlusIcon className="mr-1 size-4" />
                      Добави таг
                    </Button>
                  </DialogTrigger>
                  <DialogContent>
                    <DialogHeader>
                      <DialogTitle>Нов таг</DialogTitle>
                    </DialogHeader>
                    <div className="space-y-4 py-2">
                      <div className="space-y-1.5">
                        <Label>Име</Label>
                        <Input
                          placeholder="напр. основно"
                          value={tagName}
                          onChange={(e) => setTagName(e.target.value)}
                          onKeyDown={(e) => e.key === "Enter" && addTag()}
                        />
                      </div>
                    </div>
                    <DialogFooter>
                      <Button onClick={addTag}>Запази</Button>
                    </DialogFooter>
                  </DialogContent>
                </Dialog>
              </CardHeader>
              <CardContent>
                <div className="flex flex-wrap gap-2">
                  {tags.length === 0 && (
                    <p className="w-full py-8 text-center text-muted-foreground">
                      Няма тагове
                    </p>
                  )}
                  {tags.map((tag) => {
                    const count = expenses.filter((e) =>
                      e.tagIds.includes(tag.id)
                    ).length
                    return (
                      <div
                        key={tag.id}
                        className="flex items-center gap-1.5 rounded-full border py-1 pr-1 pl-3"
                      >
                        <TagIcon className="size-3 text-muted-foreground" />
                        <span className="text-sm">{tag.name}</span>
                        <Badge
                          variant="secondary"
                          className="rounded-full text-xs"
                        >
                          {count}
                        </Badge>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="size-5 rounded-full text-muted-foreground hover:text-destructive"
                          onClick={() => deleteTag(tag.id)}
                        >
                          <XIcon className="size-3" />
                        </Button>
                      </div>
                    )
                  })}
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </div>
    </div>
  )
}
