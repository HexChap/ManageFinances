import { Route, Routes } from "react-router-dom"
import { Login } from "./pages/Login"
import { SignUp } from "./pages/SignUp"
import { Landing } from "./pages/Landing"
import { Finances } from "./pages/Finances"

function App() {
  return (
    <Routes>
      <Route element={<SignUp />} path="/signup" />
      <Route element={<Login />} path="/login" />
      <Route element={<Landing />} path="/" />
      <Route element={<Finances />} path="/finances" />
    </Routes>
  )
}

export default App
