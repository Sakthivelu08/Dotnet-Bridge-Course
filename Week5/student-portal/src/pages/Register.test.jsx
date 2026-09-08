import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import Register from './Register';
import api from '../services/api';

// Mock api service
vi.mock('../services/api', () => ({
  default: {
    post: vi.fn()
  }
}));

describe('Register Form Validation tests', () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

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

  it('should enable submit button once all fields are validly filled and submit form', async () => {
    api.post.mockResolvedValueOnce({ data: {} });
    const { container } = renderRegister();
    
    const nameInput = container.querySelector('input[name="name"]');
    const dobInput = container.querySelector('input[name="dob"]');
    const designationInput = container.querySelector('input[name="designation"]');
    const emailInput = container.querySelector('input[name="email"]');
    const passwordInput = container.querySelector('input[name="password"]');
    const roleSelect = container.querySelector('select[name="role"]');
    const registerButton = screen.getByRole('button', { name: /register/i });

    fireEvent.change(nameInput, { target: { value: 'Alex Smith' } });
    fireEvent.change(dobInput, { target: { value: '2000-01-01' } });
    fireEvent.change(designationInput, { target: { value: 'Student' } });
    fireEvent.change(emailInput, { target: { value: 'alex@example.com' } });
    fireEvent.change(passwordInput, { target: { value: 'securepass123' } });
    fireEvent.change(roleSelect, { target: { name: 'role', value: '1' } });

    expect(registerButton).toBeEnabled();

    fireEvent.click(registerButton);

    await waitFor(() => {
      expect(api.post).toHaveBeenCalledWith('/auth/register', {
        name: 'Alex Smith',
        dob: '2000-01-01',
        designation: 'Student',
        email: 'alex@example.com',
        password: 'securepass123',
        role: 1
      });
    });
  });

  it('should display error message when registration fails', async () => {
    api.post.mockRejectedValueOnce({
      response: { data: { error: 'Email is already registered' } }
    });

    const { container } = renderRegister();
    const nameInput = container.querySelector('input[name="name"]');
    const dobInput = container.querySelector('input[name="dob"]');
    const designationInput = container.querySelector('input[name="designation"]');
    const emailInput = container.querySelector('input[name="email"]');
    const passwordInput = container.querySelector('input[name="password"]');
    const registerButton = screen.getByRole('button', { name: /register/i });

    fireEvent.change(nameInput, { target: { value: 'Alex Smith' } });
    fireEvent.change(dobInput, { target: { value: '2000-01-01' } });
    fireEvent.change(designationInput, { target: { value: 'Student' } });
    fireEvent.change(emailInput, { target: { value: 'alex@example.com' } });
    fireEvent.change(passwordInput, { target: { value: 'securepass123' } });
    fireEvent.click(registerButton);

    await waitFor(() => {
      expect(screen.getByText('Email is already registered')).toBeInTheDocument();
    });
  });
});
