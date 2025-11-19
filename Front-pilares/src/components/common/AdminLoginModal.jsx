import React, { useState } from 'react';
import './AdminLoginModal.css';

const AdminLoginModal = ({ isOpen, onClose, onLogin }) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    onLogin(username, password);
  };

  if (!isOpen) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="admin-modal" onClick={e => e.stopPropagation()}>
        <button className="modal-close" onClick={onClose}>×</button>
        <div className="modal-icon">
          <svg viewBox="0 0 24 24" fill="currentColor">
            <circle cx="12" cy="8" r="3.5" />
            <rect x="6" y="15" width="12" height="5" rx="2.5" />
            <circle cx="18" cy="18" r="2.5" />
            <circle cx="6" cy="18" r="2.5" />
            <circle cx="12" cy="18" r="2.5" />
            <rect x="14.25" y="13.25" width="3.5" height="2.5" rx="1.25" transform="rotate(-45 16 14.5)" />
          </svg>
        </div>
        <h2 className="modal-title">Ingresa contraseña como Administrador</h2>
        <form className="modal-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Nombre *</label>
            <input
              type="text"
              value={username}
              onChange={e => setUsername(e.target.value)}
              required
            />
          </div>
          <div className="form-group">
            <label>Contraseña *</label>
            <input
              type="password"
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
            />
          </div>
          <button type="submit" className="modal-submit-btn">Ingresar</button>
        </form>
      </div>
    </div>
  );
};

export default AdminLoginModal;
