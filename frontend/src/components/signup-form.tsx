import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
} from '@/components/ui/field'
import { Input } from '@/components/ui/input'
import { useAuth } from '@/hooks/useAuth'

export const SignupForm = ({
  className,
  ...props
}: React.ComponentProps<'div'>) => {
  const { register, login } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (password !== confirmPassword) {
      setError('Паролите не съвпадат.')
      return
    }
    setError(null)
    setLoading(true)
    try {
      await register(email, password)
      await login(email, password)
      navigate('/finances')
    } catch {
      setError('Регистрацията не успя. Опитайте с друг имейл.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className={cn('flex flex-col gap-6', className)} {...props}>
      <Card>
        <CardHeader className="text-center">
          <CardTitle className="text-xl">Създайте акаунт</CardTitle>
          <CardDescription>
            Въведете имейла си по-долу, за да създадете акаунт
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit}>
            <FieldGroup>
              <Field>
                <FieldLabel htmlFor="email">Имейл</FieldLabel>
                <Input
                  id="email"
                  type="email"
                  placeholder="m@example.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </Field>
              <Field>
                <div className="grid grid-cols-2 gap-4">
                  <Field>
                    <FieldLabel htmlFor="password">Парола</FieldLabel>
                    <Input
                      id="password"
                      type="password"
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                      required
                    />
                  </Field>
                  <Field>
                    <FieldLabel htmlFor="confirm-password">Потвърди</FieldLabel>
                    <Input
                      id="confirm-password"
                      type="password"
                      value={confirmPassword}
                      onChange={(e) => setConfirmPassword(e.target.value)}
                      required
                    />
                  </Field>
                </div>
                <FieldDescription>Трябва да е поне 6 символа.</FieldDescription>
              </Field>
              {error && (
                <p className="text-sm text-destructive">{error}</p>
              )}
              <Field>
                <Button type="submit" disabled={loading}>
                  {loading ? 'Създаване…' : 'Създай акаунт'}
                </Button>
                <FieldDescription className="text-center">
                  Вече имате акаунт?{' '}
                  <a href="/login" className="underline underline-offset-4">
                    Влезте
                  </a>
                </FieldDescription>
              </Field>
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
      <FieldDescription className="px-6 text-center">
        Като продължите, вие се съгласявате с нашите{' '}
        <a href="#">Условия за ползване</a> и{' '}
        <a href="#">Политика за поверителност</a>.
      </FieldDescription>
    </div>
  )
}
