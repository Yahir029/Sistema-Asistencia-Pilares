import React from 'react';
import './Input.css';

const Input = ({ 
  label,
  type = 'text',
  placeholder,
  value,
  onChange,
  error,
  fullWidth = false
}) => {
  return (
    <div className={`input-wrapper ${fullWidth ? 'input-wrapper-full' : ''}`}>
      {label && <label className="input-label">{label}</label>}
      <input
        type={type}
        className={`pilares-input ${error ? 'input-error' : ''}`}
        placeholder={placeholder}
        value={value}
        onChange={onChange}
      />
      {error && <span className="input-error-message">{error}</span>}
    </div>
  );
};

export default Input;
