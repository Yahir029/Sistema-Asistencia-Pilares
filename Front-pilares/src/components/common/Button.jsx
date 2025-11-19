import React from 'react';
import './Button.css';

const Button = ({ 
  children, 
  variant = 'primary', 
  size = 'medium',
  fullWidth = false,
  onClick,
  type = 'button'
}) => {
  return (
    <button
      type={type}
      className={`pilares-button button-${variant} button-${size} ${fullWidth ? 'button-full' : ''}`}
      onClick={onClick}
    >
      {children}
    </button>
  );
};

export default Button;
