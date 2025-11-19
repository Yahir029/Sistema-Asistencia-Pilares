import React, { useEffect } from 'react';
import './ErrorModal.css';

const ErrorModal = ({ isOpen, onClose, message, autoCloseDelay = 3000 }) => {
  // Auto-cierre después del delay especificado
  useEffect(() => {
    if (isOpen) {
      const timer = setTimeout(() => {
        onClose();
      }, autoCloseDelay);

      // Limpiar el timer si el componente se desmonta o isOpen cambia
      return () => clearTimeout(timer);
    }
  }, [isOpen, onClose, autoCloseDelay]);

  if (!isOpen) return null;
  
  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="error-modal" onClick={e => e.stopPropagation()}>
        <div className="error-circle">
          <svg viewBox="0 0 60 60">
            <circle 
              cx="30" 
              cy="30" 
              r="28" 
              fill="#e74c3c" 
              stroke="#c0392b" 
              strokeWidth="4"
            />
            <line
              x1="20"
              y1="20"
              x2="40"
              y2="40"
              stroke="#fff"
              strokeWidth="5"
              strokeLinecap="round"
            />
            <line
              x1="40"
              y1="20"
              x2="20"
              y2="40"
              stroke="#fff"
              strokeWidth="5"
              strokeLinecap="round"
            />
          </svg>
        </div>
        <div className="error-title">Error.</div>
        <div className="error-msg">{message || "Ha ocurrido un error"}</div>
        <button className="error-retry-btn" onClick={onClose}>
          Intentar Nuevamente
        </button>
      </div>
    </div>
  );
};

export default ErrorModal;
