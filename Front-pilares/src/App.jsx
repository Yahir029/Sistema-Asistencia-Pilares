import React from 'react';
import { BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import './App.css';
import Header from './components/common/Header';
import Footer from './components/common/Footer';
import AttendanceHome from './components/attendance/AttendanceHome';
import EmployeeList from './components/employees/EmployeeList'; // Importado
import ReportsHome from './components/reports/ReportsHome';     // Importado

function App() {
  return (
    <Router>
      <div className="app">
        <Header />

        <main className="main-content">
          <Routes>
            {/* 1. Ruta principal: Registro de Asistencia */}
            <Route path="/" element={<AttendanceHome />} />
            
            {/* 2. Ruta de Reportes: Se accederá después del login exitoso */}
            <Route path="/reports" element={<ReportsHome />} />
            
            {/* 3. Ruta de Lista de Empleados */}
            <Route path="/employees" element={<EmployeeList />} />

            {/* Opcional: Puedes agregar una ruta de fallback para 404 */}
            <Route path="*" element={<h1>404: Página no encontrada</h1>} />
            <Route path="/empleados" element={<EmployeeList />} />
            <Route path="/reportes" element={<ReportsHome />} />
          </Routes>
        </main>
        <Footer />
      </div>
    </Router>
  );
}

export default App;
