import React from 'react';
import './ConfirmModal.css';

const ConfirmModal = ({ isOpen, onClose, onConfirm, message, employeeId }) => {
  if (!isOpen) return null;

  const handleConfirm = () => {
    onConfirm();
    onClose();
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="confirm-modal" onClick={e => e.stopPropagation()}>
        <h2 className="confirm-title">
          {message || `¿Estas seguro de eliminar a ${employeeId}?`}
        </h2>
        
        <div className="confirm-buttons">
          <button 
            className="confirm-btn confirm-yes"
            onClick={handleConfirm}
          >
            SI
          </button>
          <button 
            className="confirm-btn confirm-no"
            onClick={onClose}
          >
            NO
          </button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmModal;
