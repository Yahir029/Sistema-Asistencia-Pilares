import React from 'react';
import './LoadingSpinner.css';

const LoadingSpinner = ({ 
  size = 'medium', 
  text = 'Cargando...', 
  fullScreen = false 
}) => {
  const spinnerClass = `spinner spinner-${size}`;

  if (fullScreen) {
    // Modo modal (pantalla completa con overlay)
    return (
      <div className="loading-modal-overlay">
        <div className="loading-modal-content">
          <div className={spinnerClass}></div>
          <div className="loading-text">{text}</div>
        </div>
      </div>
    );
  }

  // Modo normal (inline)
  return (
    <div className="loading-content">
      <div className={spinnerClass}></div>
      <div className="loading-text">{text}</div>
    </div>
  );
};

export default LoadingSpinner;
