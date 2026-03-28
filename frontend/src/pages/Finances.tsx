import { useState } from "react"
import {
  PlusIcon,
  Trash2Icon,
  FolderIcon,
  TagIcon,
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
import { useExpenses } from "@/hooks/useExpenses"
import { useIncomes } from "@/hooks/useIncomes"
import { useCategories } from "@/hooks/useCategories"
import { useTags } from "@/hooks/useTags"

const MOCK_USER_ID = 1

export const Finances = () => {
  const {
    expenses,
    loading: expLoading,
    error: expError,
    add: addExpense,
    remove: removeExpense,
  } = useExpenses(MOCK_USER_ID)

  const {
    incomes,
    loading: incLoading,
    error: incError,
    add: addIncome,
    remove: removeIncome,
  } = useIncomes(MOCK_USER_ID)

  const {
    categories,
    loading: catLoading,
    error: catError,
    add: addCategory,
    remove: removeCategory,
  } = useCategories(MOCK_USER_ID)

  const {
    tags,
    loading: tagLoading,
    error: tagError,
    add: addTag,
    remove: removeTag,
  } = useTags(MOCK_USER_ID)

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

  const handleAddCategory = async () => {
    if (!catName.trim()) return
    await addCategory({ name: catName.trim(), userId: MOCK_USER_ID })
    setCatName("")
    setCatOpen(false)
  }

  const handleAddTag = async () => {
    if (!tagName.trim()) return
    await addTag({ name: tagName.trim(), userId: MOCK_USER_ID })
    setTagName("")
    setTagOpen(false)
  }

  const handleAddExpense = async () => {
    const val = parseFloat(expValue)
    if (isNaN(val) || !expCatId) return
    await addExpense({
      value: val,
      categoryId: parseInt(expCatId),
      userId: MOCK_USER_ID,
      tagIds: expTagIds,
    })
    setExpValue("")
    setExpCatId("")
    setExpTagIds([])
    setExpOpen(false)
  }

  const handleAddIncome = async () => {
    const val = parseFloat(incValue)
    if (isNaN(val)) return
    await addIncome({ value: val, userId: MOCK_USER_ID })
    setIncValue("")
    setIncOpen(false)
  }

  const toggleExpTag = (id: number) =>
    setExpTagIds((ids) =>
      ids.includes(id) ? ids.filter((x) => x !== id) : [...ids, id]
    )

  const getCategoryName = (id: number) =>
    categories.find((c) => c.id === id)?.name ?? "—"

  const getTagName = (id: number) => tags.find((t) => t.id === id)?.name ?? ""

  const totalExpenses = expenses.reduce((s, e) => s + e.value, 0)
  const totalIncomes = incomes.reduce((s, i) => s + i.value, 0)
  const balance = totalIncomes - totalExpenses

  return (
    <div className="min-h-screen bg-background p-4 sm:p-6 lg:p-8">
      <div className="mx-auto max-w-5xl space-y-6">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">
            Финансов мениджър
          </h1>
        </div>

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

        <Tabs defaultValue="expenses">
          <TabsList className="grid w-full grid-cols-4">
            <TabsTrigger value="expenses">Разходи</TabsTrigger>
            <TabsTrigger value="incomes">Приходи</TabsTrigger>
            <TabsTrigger value="categories">Категории</TabsTrigger>
            <TabsTrigger value="tags">Тагове</TabsTrigger>
          </TabsList>

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
                      {tags.length > 0 && (
                        <div className="space-y-1.5">
                          <Label>Тагове</Label>
                          <div className="flex flex-wrap gap-2">
                            {tags.map((t) => (
                              <Badge
                                key={t.id}
                                variant={
                                  expTagIds.includes(t.id)
                                    ? "default"
                                    : "outline"
                                }
                                className="cursor-pointer select-none"
                                onClick={() => toggleExpTag(t.id)}
                              >
                                {t.name}
                              </Badge>
                            ))}
                          </div>
                        </div>
                      )}
                    </div>
                    <DialogFooter>
                      <Button onClick={handleAddExpense}>Запази</Button>
                    </DialogFooter>
                  </DialogContent>
                </Dialog>
              </CardHeader>
              <CardContent className="p-0">
                {expError && (
                  <p className="px-4 py-3 text-sm text-destructive">
                    {expError}
                  </p>
                )}
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead className="w-12">ID</TableHead>
                      <TableHead>Сума</TableHead>
                      <TableHead>Категория</TableHead>
                      <TableHead>Тагове</TableHead>
                      <TableHead>Дата</TableHead>
                      <TableHead className="w-12"></TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {expLoading && (
                      <TableRow>
                        <TableCell
                          colSpan={6}
                          className="py-8 text-center text-muted-foreground"
                        >
                          Зареждане…
                        </TableCell>
                      </TableRow>
                    )}
                    {!expLoading && expenses.length === 0 && (
                      <TableRow>
                        <TableCell
                          colSpan={6}
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
                        <TableCell className="text-xs text-muted-foreground">
                          {new Date(exp.createdAt).toLocaleDateString("bg-BG")}
                        </TableCell>
                        <TableCell>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="size-8 text-muted-foreground hover:text-destructive"
                            onClick={() => removeExpense(exp.id)}
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
                      <Button onClick={handleAddIncome}>Запази</Button>
                    </DialogFooter>
                  </DialogContent>
                </Dialog>
              </CardHeader>
              <CardContent className="p-0">
                {incError && (
                  <p className="px-4 py-3 text-sm text-destructive">
                    {incError}
                  </p>
                )}
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead className="w-12">ID</TableHead>
                      <TableHead>Сума</TableHead>
                      <TableHead>Дата</TableHead>
                      <TableHead className="w-12"></TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {incLoading && (
                      <TableRow>
                        <TableCell
                          colSpan={4}
                          className="py-8 text-center text-muted-foreground"
                        >
                          Зареждане…
                        </TableCell>
                      </TableRow>
                    )}
                    {!incLoading && incomes.length === 0 && (
                      <TableRow>
                        <TableCell
                          colSpan={4}
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
                        <TableCell className="text-xs text-muted-foreground">
                          {new Date(inc.createdAt).toLocaleDateString("bg-BG")}
                        </TableCell>
                        <TableCell>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="size-8 text-muted-foreground hover:text-destructive"
                            onClick={() => removeIncome(inc.id)}
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
                          onKeyDown={(e) =>
                            e.key === "Enter" && handleAddCategory()
                          }
                        />
                      </div>
                    </div>
                    <DialogFooter>
                      <Button onClick={handleAddCategory}>Запази</Button>
                    </DialogFooter>
                  </DialogContent>
                </Dialog>
              </CardHeader>
              <CardContent>
                {catError && (
                  <p className="py-2 text-sm text-destructive">{catError}</p>
                )}
                <div className="space-y-2">
                  {catLoading && (
                    <p className="py-8 text-center text-muted-foreground">
                      Зареждане…
                    </p>
                  )}
                  {!catLoading && categories.length === 0 && (
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
                          onClick={() => removeCategory(cat.id)}
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
                          onKeyDown={(e) => e.key === "Enter" && handleAddTag()}
                        />
                      </div>
                    </div>
                    <DialogFooter>
                      <Button onClick={handleAddTag}>Запази</Button>
                    </DialogFooter>
                  </DialogContent>
                </Dialog>
              </CardHeader>
              <CardContent>
                {tagError && (
                  <p className="py-2 text-sm text-destructive">{tagError}</p>
                )}
                <div className="flex flex-wrap gap-2">
                  {tagLoading && (
                    <p className="w-full py-8 text-center text-muted-foreground">
                      Зареждане…
                    </p>
                  )}
                  {!tagLoading && tags.length === 0 && (
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
                          onClick={() => removeTag(tag.id)}
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
