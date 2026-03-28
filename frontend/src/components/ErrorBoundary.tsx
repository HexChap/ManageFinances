import { Component, type ReactNode } from "react"
import { Button } from "@/components/ui/button"

interface Props {
  children: ReactNode
}

interface State {
  error: Error | null
}

export class ErrorBoundary extends Component<Props, State> {
  state: State = { error: null }

  static getDerivedStateFromError(error: Error): State {
    return { error }
  }

  render() {
    if (this.state.error) {
      return (
        <div className="flex min-h-screen flex-col items-center justify-center gap-4 p-6 text-center">
          <h1 className="text-2xl font-semibold">Нещо се обърка</h1>
          <p className="max-w-sm text-sm text-muted-foreground">
            {this.state.error.message}
          </p>
          <Button onClick={() => this.setState({ error: null })}>
            Опитай отново
          </Button>
        </div>
      )
    }

    return this.props.children
  }
}
