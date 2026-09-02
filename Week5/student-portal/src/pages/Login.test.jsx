import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import Login from './Login';
import api from '../services/api';

// Mock api service
vi.mock('../services/api', () => ({
  default: {
    post: vi.fn()
  }
}));

describe('Login Page Tests', () => {
  beforeEach(() => {
    localStorage.clear();
    vi.restoreAllMocks();
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
    api.post.mockResolvedValueOnce({
      data: { token: 'mock_jwt_token', role: 'Student' }
    });

    const setIsAuthenticatedMock = vi.fn();
    const { container } = renderLogin(setIsAuthenticatedMock);

    const emailInput = container.querySelector('input[type="email"]');
    const passwordInput = container.querySelector('input[type="password"]');
    const submitButton = screen.getByRole('button', { name: /login/i });

    fireEvent.change(emailInput, { target: { value: 'student@example.com' } });
    fireEvent.change(passwordInput, { target: { value: 'password123' } });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(localStorage.getItem('token')).toBe('mock_jwt_token');
      expect(localStorage.getItem('role')).toBe('Student');
      expect(setIsAuthenticatedMock).toHaveBeenCalledWith(true);
    });
  });

  it('should display error message when login API call fails', async () => {
    api.post.mockRejectedValueOnce({
      response: { data: { error: 'Invalid credentials.' } }
    });

    const { container } = renderLogin();
    const emailInput = container.querySelector('input[type="email"]');
    const passwordInput = container.querySelector('input[type="password"]');
    const submitButton = screen.getByRole('button', { name: /login/i });

    fireEvent.change(emailInput, { target: { value: 'wrong@example.com' } });
    fireEvent.change(passwordInput, { target: { value: 'wrongpass' } });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText('Invalid credentials.')).toBeInTheDocument();
    });
  });
});
