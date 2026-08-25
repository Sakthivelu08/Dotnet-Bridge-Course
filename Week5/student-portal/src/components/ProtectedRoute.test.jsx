import React from 'react';
import { render, screen } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { describe, it, expect, beforeEach } from 'vitest';
import ProtectedRoute from './ProtectedRoute';

describe('ProtectedRoute Component Tests', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('should redirect to /login when token is not present in localStorage', () => {
    // Render ProtectedRoute with mock child component
    render(
      <BrowserRouter>
        <ProtectedRoute>
          <div>Protected Content</div>
        </ProtectedRoute>
      </BrowserRouter>
    );

    // Protected content should NOT be in the document
    expect(screen.queryByText('Protected Content')).not.toBeInTheDocument();
  });

  it('should render children when token is present in localStorage', () => {
    localStorage.setItem('token', 'mock_jwt_token_value');

    render(
      <BrowserRouter>
        <ProtectedRoute>
          <div>Protected Content</div>
        </ProtectedRoute>
      </BrowserRouter>
    );

    // Protected content should be successfully displayed
    expect(screen.getByText('Protected Content')).toBeInTheDocument();
  });
});
