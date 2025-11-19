import React, { useState, useMemo } from "react";
import "./ReportsHome.css";
import { FaEye, FaDownload } from "react-icons/fa";
import { useNavigate } from "react-router-dom";

const ReportsHome = () => {
  // Estado de filtros / parámetros
  const navigate = useNavigate();  

  const [showEmployeeModal, setShowEmployeeModal] = useState(false);

  // Datos mock de empleados (luego se reemplaza por API)
  const employees = [
    { id: 1, name: "Melanie Gamero" },
    { id: 2, name: "Ana Martínez" },
    { id: 3, name: "Carlos Gómez" },
    { id: 4, name: "Lucía Fernández" },
    { id: 5, name: "Sofía Ruiz" },
  ];

  const [filters, setFilters] = useState({
    search: "",
    fechaEntrada1: "",
    fechaEntrada2: "",
    horaEntrada: "",
    horaSalida: "",
    horasTrabajadas: "",
  });

  // Historial inicial (tal como en tu mock)
  const [historial, setHistorial] = useState([
    { id: 1, nombre: "ReporteTodosMayo", fecha: "31/12/2023", parametros: {} },
    { id: 2, nombre: "ReporteMelanieJunio", fecha: "31/03/2024", parametros: {} },
    { id: 3, nombre: "ResumenLuciaAbril", fecha: "30/04/2024", parametros: {} },
    { id: 4, nombre: "ReporteCarlosEnero", fecha: "31/05/2024", parametros: {} },
    { id: 5, nombre: "ReporteAnaFebrero", fecha: "15/06/2024", parametros: {} },
  ]);

  // Manejo de cambios en inputs (usa el atributo name en cada input)
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFilters((prev) => ({ ...prev, [name]: value }));
  };

  // Generar reporte (simulado): valida y agrega al historial
  const handleGenerarReporte = () => {
    // Validación básica: fechas obligatorias (puedes ajustar)
    if (!filters.fechaEntrada1 || !filters.fechaEntrada2) {
      alert("Selecciona fecha de entrada (ambas) antes de generar el reporte.");
      return;
    }

    const nuevo = {
      id: historial.length + 1,
      nombre: `Reporte${historial.length + 1}`,
      fecha: new Date().toLocaleDateString(),
      parametros: { ...filters },
    };

    setHistorial((prev) => [nuevo, ...prev]);
    alert("Reporte generado (simulado). Se agregó al historial.");
  };

  // Ver reporte: muestra parámetros en alerta (simulado)
  const handleVer = (item) => {
    const p = item.parametros && Object.keys(item.parametros).length
      ? item.parametros
      : filters; // si no hay params guardados, mostramos filtros actuales
    alert(`Vista previa (simulada) - ${item.nombre}\n\n` + JSON.stringify(p, null, 2));
  };

  // Descargar reporte (simulado)
  const handleDescargar = (item) => {
    alert(`Descargando (simulado): ${item.nombre}`);
    // Aquí podrías generar un blob/pdf real luego
  };

  // Filtro simple de búsqueda en historial (por nombre)
  const historialFiltrado = useMemo(() => {
    const q = filters.search?.trim().toLowerCase();
    if (!q) return historial;
    return historial.filter((h) => h.nombre.toLowerCase().includes(q));
  }, [filters.search, historial]);

  return (
    <div className="reports-home">
      <h2 className="title-main">Reportes e Historial</h2>

      <div className="control-panel-right">
        <button 
          className="back-button"
          onClick={() => navigate('/employees')}
        >
          ← Volver
        </button>
      </div>

      {/* --- MODAL DE EMPLEADOS --- */}
      {showEmployeeModal && (
        <div className="modal-overlay">
          <div className="modal">
            <div className="modal-header">
              <h3>Empleados</h3>
              <button className="modal-close" onClick={() => setShowEmployeeModal(false)}>
                ✖
              </button>
            </div>

            <div className="modal-content">
              <div className="employee-grid">
                {employees.map(emp => (
                  <button key={emp.id} className="employee-item">
                    {emp.name}
                  </button>
                ))}
              </div>
            </div>

            <div className="modal-footer">
              <button className="modal-save">Guardar</button>
            </div>
          </div>
        </div>
      )}


      <div className="main-grid">
        {/* PANEL IZQUIERDO */}
        <div className="left-panel">
          <h3 className="panel-title">Parámetros</h3>

          {/* BUSCADOR */}
          <div className="search-row">
            <div className="tag">Todos</div>
            <input
              className="search-input"
              placeholder="Buscar por Nombre o ID"
              name="search"
              value={filters.search}
              onChange={handleChange}
            />
            <button
              className="btn-search"
              onClick={() => setShowEmployeeModal(true)}>
              Buscar
            </button>
          </div>

          {/* FILTROS */}
          <div className="filters-grid">
            <div className="input-group">
              <label>Fecha de entrada</label>
              <input
                type="date"
                name="fechaEntrada1"
                value={filters.fechaEntrada1}
                onChange={handleChange}
              />
            </div>

            <div className="input-group">
              <label>Hora de entrada</label>
              <input
                type="time"
                name="horaEntrada"
                value={filters.horaEntrada}
                onChange={handleChange}
              />
            </div>

            <div className="input-group">
              <label>Fecha de entrada</label>
              <input
                type="date"
                name="fechaEntrada2"
                value={filters.fechaEntrada2}
                onChange={handleChange}
              />
            </div>

            <div className="input-group">
              <label>Hora de salida</label>
              <input
                type="time"
                name="horaSalida"
                value={filters.horaSalida}
                onChange={handleChange}
              />
            </div>
          </div>

          <div className="input-group full">
            <label>Horas totales trabajadas</label>
            <input
              type="text"
              name="horasTrabajadas"
              placeholder="Horas totales trabajadas"
              value={filters.horasTrabajadas}
              onChange={handleChange}
            />
          </div>

          <button className="btn-report" onClick={handleGenerarReporte}>
            Generar reporte
          </button>
        </div>

        {/* PANEL DERECHO (vista previa) */}
        <div className="right-panel">
          <h3 className="panel-title">Parámetros</h3>

          <div style={{ marginTop: 10 }}>
            <p><strong>Busqueda:</strong> {filters.search || "N/A"}</p>
            <p><strong>Fecha entrada (1):</strong> {filters.fechaEntrada1 || "N/A"}</p>
            <p><strong>Hora entrada:</strong> {filters.horaEntrada || "N/A"}</p>
            <p><strong>Fecha entrada (2):</strong> {filters.fechaEntrada2 || "N/A"}</p>
            <p><strong>Hora salida:</strong> {filters.horaSalida || "N/A"}</p>
            <p><strong>Horas trabajadas:</strong> {filters.horasTrabajadas || "N/A"}</p>
          </div>
        </div>
      </div>

      {/* TABLA */}
      <div className="table-container">
        <table>
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Fecha</th>
              <th>Ver</th>
              <th>Descargar</th>
            </tr>
          </thead>

          <tbody>
            {historialFiltrado.map((h) => (
              <tr key={h.id}>
                <td>{h.nombre}</td>
                <td>{h.fecha}</td>
                <td onClick={() => handleVer(h)} style={{ cursor: "pointer", textAlign: "center" }}>
                  <FaEye className="icon" />
                </td>
                <td onClick={() => handleDescargar(h)} style={{ cursor: "pointer", textAlign: "center" }}>
                  <FaDownload className="icon" />
                </td>
              </tr>
            ))}

            {historialFiltrado.length === 0 && (
              <tr>
                <td colSpan={4} style={{ textAlign: "center", padding: 12 }}>
                  No se encontraron reportes.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default ReportsHome;
