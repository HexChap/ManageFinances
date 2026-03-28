import { Route, Routes } from "react-router-dom"
import { Login } from "./pages/Login"
import { SignUp } from "./pages/SignUp"
import { Landing } from "./pages/Landing"

function App() {
  return (
    <Routes>
      <Route element={<SignUp />} path="/signup" />
      <Route element={<Login />} path="/login" />
      <Route element={<Landing />} path="/" />
    </Routes>
  )
}

export default App
