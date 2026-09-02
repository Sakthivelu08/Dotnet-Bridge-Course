import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import Students from './Students';
import api from '../services/api';

// Mock api service
vi.mock('../services/api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    delete: vi.fn()
  }
}));

describe('Students Directory Dashboard Tests', () => {
  beforeEach(() => {
    localStorage.clear();
    vi.restoreAllMocks();
  });

  it('should hide Write/Create controls and render student list when role is Student', async () => {
    api.get.mockResolvedValueOnce({
      data: [
        { id: 1, name: 'Sakthivelu S', dob: '2003-10-08', designation: '12th', email: 'svelu107@gmail.com' },
        { id: 2, name: 'Jane Doe', dob: '2001-05-15', designation: 'Developer', email: 'jane@example.com' }
      ]
    });

    localStorage.setItem('role', 'Student');
    render(<Students />);

    await waitFor(() => {
      expect(screen.getByText('Sakthivelu S')).toBeInTheDocument();
    });

    expect(screen.queryByText(/Add New Student Record/i)).not.toBeInTheDocument();
  });

  it('should show Write/Create controls, support adding students, and support deletion when role is Teacher', async () => {
    api.get.mockResolvedValueOnce({
      data: [
        { id: 1, name: 'Sakthivelu S', dob: '2003-10-08', designation: '12th', email: 'svelu107@gmail.com' }
      ]
    });
    api.post.mockResolvedValueOnce({ data: {} });
    api.delete.mockResolvedValueOnce({ data: {} });

    localStorage.setItem('role', 'Teacher');
    const { container } = render(<Students />);

    await waitFor(() => {
      expect(screen.getByText('Add New Student Record (Teacher Only)')).toBeInTheDocument();
    });

    const nameInput = container.querySelector('input[name="name"]');
    const dobInput = container.querySelector('input[name="dob"]');
    const designationInput = container.querySelector('input[name="designation"]');
    const emailInput = container.querySelector('input[name="email"]');
    const addButton = screen.getByRole('button', { name: /add/i });

    fireEvent.change(nameInput, { target: { value: 'New Student' } });
    fireEvent.change(dobInput, { target: { value: '2005-05-05' } });
    fireEvent.change(designationInput, { target: { value: '11th' } });
    fireEvent.change(emailInput, { target: { value: 'new@example.com' } });

    fireEvent.click(addButton);

    await waitFor(() => {
      expect(api.post).toHaveBeenCalledWith('/students', {
        name: 'New Student',
        dob: '2005-05-05',
        designation: '11th',
        email: 'new@example.com'
      });
    });

    const deleteButtons = screen.getAllByRole('button', { name: /delete/i });
    fireEvent.click(deleteButtons[0]);

    await waitFor(() => {
      expect(api.delete).toHaveBeenCalledWith('/students/1');
    });
  });

  it('should display error message when fetching students fails', async () => {
    api.get.mockRejectedValueOnce(new Error('Network error'));
    render(<Students />);

    await waitFor(() => {
      expect(screen.getByText('Could not load student records from the database.')).toBeInTheDocument();
    });
  });

  it('should handle errors gracefully when create or delete fails', async () => {
    const alertSpy = vi.spyOn(window, 'alert').mockImplementation(() => {});
    
    api.get.mockResolvedValueOnce({
      data: [{ id: 1, name: 'Test Student', dob: '2000-01-01', designation: '10th', email: 'test@example.com' }]
    });
    api.post.mockRejectedValueOnce(new Error('Post error'));
    api.delete.mockRejectedValueOnce(new Error('Delete error'));

    localStorage.setItem('role', 'Teacher');
    const { container } = render(<Students />);

    await waitFor(() => {
      expect(screen.getByText('Test Student')).toBeInTheDocument();
    });

    const nameInput = container.querySelector('input[name="name"]');
    const dobInput = container.querySelector('input[name="dob"]');
    const designationInput = container.querySelector('input[name="designation"]');
    const emailInput = container.querySelector('input[name="email"]');
    const addButton = screen.getByRole('button', { name: /add/i });

    fireEvent.change(nameInput, { target: { value: 'New Fail' } });
    fireEvent.change(dobInput, { target: { value: '2005-05-05' } });
    fireEvent.change(designationInput, { target: { value: '11th' } });
    fireEvent.change(emailInput, { target: { value: 'fail@example.com' } });

    fireEvent.click(addButton);

    await waitFor(() => {
      expect(alertSpy).toHaveBeenCalledWith('Could not add new student.');
    });

    const deleteButton = screen.getByRole('button', { name: /delete/i });
    fireEvent.click(deleteButton);

    await waitFor(() => {
      expect(alertSpy).toHaveBeenCalledWith('Delete operation failed.');
    });
  });
});
