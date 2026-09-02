import { describe, it, expect, vi, beforeEach } from 'vitest';
import api from './api';

describe('Axios API Service and Interceptors', () => {
  beforeEach(() => {
    localStorage.clear();
    vi.restoreAllMocks();
    
    // Mock window.location to prevent JSDOM navigation warnings
    Object.defineProperty(window, 'location', {
      writable: true,
      value: { href: 'http://localhost/students', pathname: '/students' }
    });
  });

  it('should attach Authorization header when token is present in localStorage', async () => {
    localStorage.setItem('token', 'sample_token_123');
    
    const config = { headers: {} };
    const requestHandler = api.interceptors.request.handlers[0].fulfilled;
    const updatedConfig = requestHandler(config);

    expect(updatedConfig.headers.Authorization).toBe('Bearer sample_token_123');
  });

  it('should handle 401 response by clearing localStorage', async () => {
    localStorage.setItem('token', 'sample_token_123');
    localStorage.setItem('role', 'Teacher');

    const errorHandler = api.interceptors.response.handlers[0].rejected;
    const error401 = { response: { status: 401 } };

    await expect(errorHandler(error401)).rejects.toBeDefined();
    expect(localStorage.getItem('token')).toBeNull();
    expect(localStorage.getItem('role')).toBeNull();
  });

  it('should handle 403 Forbidden response with custom message', async () => {
    const errorHandler = api.interceptors.response.handlers[0].rejected;
    const error403 = { response: { status: 403 } };

    try {
      await errorHandler(error403);
    } catch (err) {
      expect(err.message).toContain('Access Denied');
    }
  });

  it('should handle 400 Bad Request and 500 Server Error responses', async () => {
    const errorHandler = api.interceptors.response.handlers[0].rejected;
    
    const error400 = { response: { status: 400, data: { error: 'Validation failed' } } };
    try {
      await errorHandler(error400);
    } catch (err) {
      expect(err.message).toBe('Validation failed');
    }

    const error500 = { response: { status: 500 } };
    try {
      await errorHandler(error500);
    } catch (err) {
      expect(err.message).toContain('Server error encountered');
    }
  });
});
