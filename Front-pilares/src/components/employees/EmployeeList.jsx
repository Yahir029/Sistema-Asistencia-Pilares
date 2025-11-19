import React, { useMemo, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './EmployeeList.css';
import ConfirmModal from './../common/ConfirmModal';
import SuccessModal from './../common/SuccessModal';

const API_BASE_URL = 'http://localhost:5172';

const EmployeeList = () => {
  const navigate = useNavigate();
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [filterActive, setFilterActive] = useState('all');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [modalData, setModalData] = useState({
    isNew: true,
    employee: null
  });

  // Estados para confirmación y éxito
  const [showConfirm, setShowConfirm] = useState(false);
  const [employeeToDelete, setEmployeeToDelete] = useState(null);
  const [showSuccess, setShowSuccess] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  
  // Estado para el switch de administrador
  const [isAdminInModal, setIsAdminInModal] = useState(false);
  // Estado para el valor del input de Rol
  const [roleValue, setRoleValue] = useState('');

  const token = localStorage.getItem('authToken');

  // *** CARGAR EMPLEADOS DESDE EL BACKEND ***
  useEffect(() => {
    const fetchEmployees = async () => {
      setLoading(true);
      try {
        const response = await fetch(`${API_BASE_URL}/empleados`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
          },
        });

        if (response.ok) {
          const data = await response.json();
          // Transformar los datos del backend al formato del frontend
          const transformedData = data.map(emp => ({
            id: emp.idEmpleadoExterno,
            dbId: emp.id, // Guardar el ID de la base de datos
            name: emp.nombre,
            email: emp.email || '',
            phone: emp.telefono || '',
            role: emp.rolNombre || emp.rol || '',
            area: emp.areaNombre || emp.area || '',
            active: emp.estaActivo,
            admin: emp.esAdmin,
            schedule: emp.horarios || {}
          }));
          setEmployees(transformedData);
        } else if (response.status === 401) {
          alert('No autorizado. Por favor inicia sesión nuevamente.');
          navigate('/');
        } else {
          alert('Error al cargar empleados');
        }
      } catch (error) {
        console.error('Error:', error);
        alert('No se pudo conectar al servidor');
      }
      setLoading(false);
    };

    if (token) {
      fetchEmployees();
    } else {
      navigate('/');
    }
  }, [token, navigate]);

  // Filtrado y búsqueda
  const filteredEmployees = useMemo(() => {
    return employees.filter(emp => {
      const matchesSearch = 
        emp.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        emp.id.toLowerCase().includes(searchQuery.toLowerCase()) ||
        emp.email.toLowerCase().includes(searchQuery.toLowerCase());

      const matchesFilter = 
        filterActive === 'all' ||
        (filterActive === 'active' && emp.active) ||
        (filterActive === 'inactive' && !emp.active);

      return matchesSearch && matchesFilter;
    });
  }, [employees, searchQuery, filterActive]);

  // Función para manejar el cambio del switch
  const handleAdminSwitchChange = (e) => {
    const isChecked = e.target.checked;
    setIsAdminInModal(isChecked);
    if (isChecked) {
      setRoleValue('Administrador');
    } else {
      setRoleValue('');
    }
  };

  // Abrir modal para agregar
  const handleAddEmployee = () => {
    setModalData({
      isNew: true,
      employee: {
        id: '',
        name: '',
        email: '',
        phone: '',
        role: '',
        area: '',
        password: '',
        schedule: {
          Lunes: { from: '', to: '' },
          Martes: { from: '', to: '' },
          Miercoles: { from: '', to: '' },
          Jueves: { from: '', to: '' },
          Viernes: { from: '', to: '' },
          Sabado: { from: '', to: '' },
          Domingo: { from: '', to: '' }
        }
      }
    });
    setIsAdminInModal(false);
    setRoleValue('');
    setIsModalOpen(true);
  };

  // Abrir modal para editar
  const handleEditEmployee = (employee) => {
    setModalData({
      isNew: false,
      employee: { ...employee }
    });
    setIsAdminInModal(employee.admin || false);
    setRoleValue(employee.role || '');
    setIsModalOpen(true);
  };

  // Cerrar modal de agregar/editar
  const toggleModal = () => {
    setIsModalOpen(false);
  };

  // *** GUARDAR EMPLEADO (AGREGAR O EDITAR) ***
  const handleSaveEmployee = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    
    const employeeData = {
      idEmpleadoExterno: formData.get('id'),
      nombre: formData.get('name'),
      email: formData.get('email'),
      telefono: formData.get('phone'),
      nombreRol: isAdminInModal ? 'Administrador' : roleValue,
      nombreArea: formData.get('area'),
      esAdmin: isAdminInModal,
      password: formData.get('password') || undefined
    };

    try {
      if (modalData.isNew) {
        // Crear nuevo empleado
        const response = await fetch(`${API_BASE_URL}/empleados`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
          },
          body: JSON.stringify(employeeData)
        });

        if (response.ok) {
          showSuccessNotification(`Empleado ${employeeData.idEmpleadoExterno} agregado correctamente`);
          // Recargar la lista
          window.location.reload();
        } else {
          const error = await response.json();
          alert(`Error: ${error.mensaje || 'No se pudo agregar el empleado'}`);
        }
      } else {
        // Actualizar empleado existente
        const response = await fetch(`${API_BASE_URL}/empleados/${modalData.employee.dbId}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
          },
          body: JSON.stringify(employeeData)
        });

        if (response.ok || response.status === 204) {
          showSuccessNotification(`Empleado ${employeeData.idEmpleadoExterno} actualizado correctamente`);
          // Recargar la lista
          window.location.reload();
        } else {
          alert('Error al actualizar el empleado');
        }
      }
      setIsModalOpen(false);
    } catch (error) {
      console.error('Error:', error);
      alert('No se pudo conectar al servidor');
    }
  };

  // Mostrar modal de confirmación para eliminar
  const handleDeleteEmployee = (id) => {
    setEmployeeToDelete(id);
    setShowConfirm(true);
  };

  // *** CONFIRMAR ELIMINACIÓN ***
  const confirmDelete = async () => {
    if (employeeToDelete) {
      const employee = employees.find(emp => emp.id === employeeToDelete);
      
      try {
        const response = await fetch(`${API_BASE_URL}/empleados/${employee.dbId}`, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${token}`,
          },
        });

        if (response.ok || response.status === 204) {
          setEmployees(employees.filter(emp => emp.id !== employeeToDelete));
          showSuccessNotification(`Empleado ${employeeToDelete} eliminado correctamente`);
        } else {
          alert('Error al eliminar el empleado');
        }
      } catch (error) {
        console.error('Error:', error);
        alert('No se pudo conectar al servidor');
      }
      
      setEmployeeToDelete(null);
      setShowConfirm(false);
    }
  };

  // Cancelar eliminación
  const cancelDelete = () => {
    setShowConfirm(false);
    setEmployeeToDelete(null);
  };

  // Mostrar modal de éxito
  const showSuccessNotification = (message) => {
    setSuccessMessage(message);
    setShowSuccess(true);
  };

  // Cerrar modal de éxito
  const handleCloseSuccess = () => {
    setShowSuccess(false);
    setSuccessMessage('');
  };

  // *** TOGGLE ESTADO ACTIVO/INACTIVO ***
  const toggleEmployeeStatus = async (id) => {
    const employee = employees.find(emp => emp.id === id);
    
    try {
      const response = await fetch(`${API_BASE_URL}/empleados/${employee.dbId}/estado?activo=${!employee.active}`, {
        method: 'PATCH',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (response.ok || response.status === 204) {
        setEmployees(employees.map(emp =>
          emp.id === id ? { ...emp, active: !emp.active } : emp
        ));
      } else {
        alert('Error al cambiar el estado del empleado');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('No se pudo conectar al servidor');
    }
  };

  if (loading) {
    return (
      <div className="employee-list-container">
        <div style={{ textAlign: 'center', padding: '50px' }}>
          <h2>Cargando empleados...</h2>
        </div>
      </div>
    );
  }

  return (
    <div className="employee-list-container">
      <h1 className="page-title">Gestión de Empleados</h1>

      {/* Panel de controles */}
      <div className="control-panel">
        <div className="control-panel-left">
          <button className="control-button" onClick={handleAddEmployee}>
            ➕ Agregar Empleado
          </button>
          <button className="control-button" onClick={() => navigate('/reports')}>
            📊 Ver Reportes
          </button>
        </div>

        <div className="control-panel-right">
          <button 
            className="back-button"
            onClick={() => navigate('/')}
          >
            ← Volver
          </button>
        </div>
      </div>

      {/* Barra de búsqueda y filtros */}
      <div className="search-filter-bar">
        <input
          type="text"
          placeholder="Buscar por nombre, ID o email..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="search-input"
        />
        
        <div className="filter-buttons">
          <button
            className={`filter-btn ${filterActive === 'all' ? 'active' : ''}`}
            onClick={() => setFilterActive('all')}
          >
            Todos
          </button>
          <button
            className={`filter-btn ${filterActive === 'active' ? 'active' : ''}`}
            onClick={() => setFilterActive('active')}
          >
            Activos
          </button>
          <button
            className={`filter-btn ${filterActive === 'inactive' ? 'active' : ''}`}
            onClick={() => setFilterActive('inactive')}
          >
            Inactivos
          </button>
        </div>
      </div>

      {/* Tabla de empleados */}
      <div className="table-container">
        <table className="employee-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Nombre</th>
              <th>Email</th>
              <th>Teléfono</th>
              <th>Rol</th>
              <th>Área</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {filteredEmployees.map((emp) => (
              <tr key={emp.id} className={emp.active ? '' : 'inactive-row'}>
                <td>{emp.id}</td>
                <td>{emp.name}</td>
                <td>{emp.email || 'N/A'}</td>
                <td>{emp.phone || 'N/A'}</td>
                <td>{emp.role}</td>
                <td>{emp.area}</td>
                <td>
                  <span className={`status-badge ${emp.active ? 'active' : 'inactive'}`}>
                    {emp.active ? 'Activo' : 'Inactivo'}
                  </span>
                </td>
                <td>
                  <div className="action-buttons">
                    <button
                      className="btn-edit"
                      onClick={() => handleEditEmployee(emp)}
                      title="Editar"
                    >
                      ✏️
                    </button>
                    <button
                      className="btn-toggle"
                      onClick={() => toggleEmployeeStatus(emp.id)}
                      title={emp.active ? 'Desactivar' : 'Activar'}
                    >
                      {emp.active ? '🔒' : '🔓'}
                    </button>
                    <button
                      className="btn-delete"
                      onClick={() => handleDeleteEmployee(emp.id)}
                      title="Eliminar"
                    >
                      🗑️
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Modal para agregar/editar empleado */}
      {isModalOpen && (
        <div className="modal-overlay" onClick={toggleModal}>
          <div className="add-employee-modal" onClick={(e) => e.stopPropagation()}>
            <h2 className="modal-title">
              {modalData.isNew ? 'Agregar nuevo empleado' : 'Editar empleado'}
            </h2>

            <form className="add-employee-form" onSubmit={handleSaveEmployee}>
              {/* Nombre completo */}
              <div className="form-group">
                <label htmlFor="name">Nombre completo *</label>
                <input
                  type="text"
                  id="name"
                  name="name"
                  defaultValue={modalData.employee?.name}
                  required
                />
              </div>

              {/* ID de empleado */}
              <div className="form-group">
                <label htmlFor="id">ID de empleado *</label>
                <input
                  type="text"
                  id="id"
                  name="id"
                  defaultValue={modalData.employee?.id}
                  required
                  disabled={!modalData.isNew}
                />
              </div>

              {/* Email y Teléfono en fila */}
              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="email">Email</label>
                  <input
                    type="email"
                    id="email"
                    name="email"
                    defaultValue={modalData.employee?.email}
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="phone">Teléfono</label>
                  <input
                    type="tel"
                    id="phone"
                    name="phone"
                    defaultValue={modalData.employee?.phone}
                  />
                </div>
              </div>

              {/* Contraseña (solo para nuevos empleados admin) */}
              {modalData.isNew && (
                <div className="form-group">
                  <label htmlFor="password">
                    Contraseña {isAdminInModal ? '*' : '(opcional)'}
                  </label>
                  <input
                    type="password"
                    id="password"
                    name="password"
                    required={isAdminInModal}
                    placeholder={isAdminInModal ? 'Requerida para administradores' : 'Dejar en blanco si no es admin'}
                  />
                </div>
              )}

              {/* Switch de Administrador */}
              <div className="form-group admin-switch-container">
                <label className="admin-switch-label">
                  <span>¿Es Administrador?</span>
                  <label className="switch">
                    <input
                      type="checkbox"
                      checked={isAdminInModal}
                      onChange={handleAdminSwitchChange}
                    />
                    <span className="slider"></span>
                  </label>
                </label>
              </div>

              {/* Rol y Área en fila */}
              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="role">Rol</label>
                  <input
                    type="text"
                    id="role"
                    name="role"
                    placeholder={isAdminInModal ? "Administrador (automático)" : "Ej: Docente, Coordinador, etc."}
                    value={isAdminInModal ? 'Administrador' : roleValue}
                    onChange={(e) => setRoleValue(e.target.value)}
                    disabled={isAdminInModal}
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="area">Área</label>
                  <input
                    type="text"
                    id="area"
                    name="area"
                    placeholder="Ej: Dirección, Cultura, etc."
                    defaultValue={modalData.employee?.area}
                  />
                </div>
              </div>

              {/* Botones */}
              <div className="modal-buttons">
                <button 
                  type="button" 
                  className="cancel-button"
                  onClick={toggleModal}
                >
                  Cancelar
                </button>
                <button 
                  type="submit" 
                  className="submit-button"
                >
                  {modalData.isNew ? 'AGREGAR' : 'GUARDAR'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Modal de confirmación de eliminación */}
      <ConfirmModal
        isOpen={showConfirm}
        onClose={cancelDelete}
        onConfirm={confirmDelete}
        message={`¿Estas seguro de eliminar a ${employeeToDelete}?`}
        employeeId={employeeToDelete}
      />

      {/* Modal de éxito */}
      <SuccessModal
        isOpen={showSuccess}
        onClose={handleCloseSuccess}
        message={successMessage}
        autoCloseDelay={3000}
      />
    </div>
  );
};

export default EmployeeList;
