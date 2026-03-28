import { Route, Routes } from "react-router-dom"
import { Login } from "./pages/Login"
import { SignUp } from "./pages/SignUp"
import { Landing } from "./pages/Landing"
import { Finances } from "./pages/Finances"
import NotFound from "./pages/NotFound"

function App() {
  return (
    <Routes>
      <Route element={<Landing />} path="/" />
      <Route element={<Login />} path="/login" />
      <Route element={<SignUp />} path="/signup" />
      <Route element={<Finances />} path="/finances" />
      <Route element={<NotFound />} path="*" />
    </Routes>
  )
}

export default App
