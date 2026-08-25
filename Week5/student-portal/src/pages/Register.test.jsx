import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { describe, it, expect, vi } from 'vitest';
import Register from './Register';

// Mock api service
vi.mock('../services/api', () => ({
  default: {
    post: vi.fn(() => Promise.resolve({ data: {} }))
  }
}));

describe('Register Form Validation tests', () => {
  const renderRegister = () => {
    return render(
      <BrowserRouter>
        <Register />
      </BrowserRouter>
    );
  };

  it('should disable submit button on initial mount', () => {
    renderRegister();
    const registerButton = screen.getByRole('button', { name: /register/i });
    expect(registerButton).toBeDisabled();
  });

  it('should enable submit button once all fields are validly filled', async () => {
    const { container } = renderRegister();
    
    // Select inputs using their unique name attributes in the form
    const nameInput = container.querySelector('input[name="name"]');
    const dobInput = container.querySelector('input[name="dob"]');
    const designationInput = container.querySelector('input[name="designation"]');
    const emailInput = container.querySelector('input[name="email"]');
    const passwordInput = container.querySelector('input[name="password"]');
    const registerButton = screen.getByRole('button', { name: /register/i });

    // Fill valid values
    fireEvent.change(nameInput, { target: { value: 'Alex Smith' } });
    fireEvent.change(dobInput, { target: { value: '2000-01-01' } });
    fireEvent.change(designationInput, { target: { value: 'Student' } });
    fireEvent.change(emailInput, { target: { value: 'alex@example.com' } });
    fireEvent.change(passwordInput, { target: { value: 'securepass123' } });

    // Assert submit button is enabled
    expect(registerButton).toBeEnabled();
  });
});
