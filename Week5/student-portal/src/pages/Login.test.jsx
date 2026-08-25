import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import Login from './Login';

// Mock api service
vi.mock('../services/api', () => ({
  default: {
    post: vi.fn(() => Promise.resolve({
      data: { token: 'mock_jwt_token', role: 'Student' }
    }))
  }
}));

describe('Login Page Tests', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  const renderLogin = (setIsAuthenticatedMock = vi.fn()) => {
    return render(
      <BrowserRouter>
        <Login setIsAuthenticated={setIsAuthenticatedMock} />
      </BrowserRouter>
    );
  };

  it('should render email and password inputs', () => {
    const { container } = renderLogin();
    expect(container.querySelector('input[type="email"]')).toBeInTheDocument();
    expect(container.querySelector('input[type="password"]')).toBeInTheDocument();
  });

  it('should call api.post and handle successful login', async () => {
    const setIsAuthenticatedMock = vi.fn();
    const { container } = renderLogin(setIsAuthenticatedMock);

    const emailInput = container.querySelector('input[type="email"]');
    const passwordInput = container.querySelector('input[type="password"]');
    const submitButton = screen.getByRole('button', { name: /login/i });

    // Simulate input typing
    fireEvent.change(emailInput, { target: { value: 'student@example.com' } });
    fireEvent.change(passwordInput, { target: { value: 'password123' } });

    // Submit form
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(localStorage.getItem('token')).toBe('mock_jwt_token');
      expect(localStorage.getItem('role')).toBe('Student');
      expect(setIsAuthenticatedMock).toHaveBeenCalledWith(true);
    });
  });
});
