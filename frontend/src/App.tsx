import { Route, Routes } from 'react-router-dom'
import { Login } from './pages/Login'
import { SignUp } from './pages/SignUp'
import { Landing } from './pages/Landing'
import { Finances } from './pages/Finances'
import NotFound from './pages/NotFound'
import { ProtectedRoute } from './components/ProtectedRoute'
import { AuthProvider } from './components/AuthContext'

function App() {
  return (
    <AuthProvider>
      <Routes>
        <Route element={<Landing />} path="/" />
        <Route element={<Login />} path="/login" />
        <Route element={<SignUp />} path="/signup" />
        <Route
          element={
            <ProtectedRoute>
              <Finances />
            </ProtectedRoute>
          }
          path="/finances"
        />
        <Route element={<NotFound />} path="*" />
      </Routes>
    </AuthProvider>
  )
}

export default App
