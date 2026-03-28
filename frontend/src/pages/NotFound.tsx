import { Link } from "react-router-dom"
import { Button } from "@/components/ui/button"

export default function NotFound() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-4 p-6 text-center">
      <p className="text-6xl font-bold text-muted-foreground/30">404</p>
      <h1 className="text-2xl font-semibold">Страницата не е намерена</h1>
      <p className="text-sm text-muted-foreground">
        Адресът, който търсиш, не съществува.
      </p>
      <Button asChild>
        <Link to="/">Към началото</Link>
      </Button>
    </div>
  )
}
