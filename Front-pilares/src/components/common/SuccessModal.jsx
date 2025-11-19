import React, { useEffect } from 'react';
import './SuccessModal.css';

const SuccessModal = ({ isOpen, onClose, message, autoCloseDelay = 3000 }) => {
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
      <div className="success-modal" onClick={e => e.stopPropagation()}>
        <div className="success-circle">
          <svg viewBox="0 0 60 60">
            <circle 
              cx="30" 
              cy="30" 
              r="28" 
              fill="#3ac045" 
              stroke="#178f27" 
              strokeWidth="4"
            />
            <polyline
              points="18,32 27,40 43,22"
              fill="none"
              stroke="#fff"
              strokeWidth="5"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
        </div>
        <div className="success-msg">{message || "Entrada registrada"}</div>
      </div>
    </div>
  );
};

export default SuccessModal;
