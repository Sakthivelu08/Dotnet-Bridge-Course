import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import Students from './Students';
import api from '../services/api';

// Mock api service
vi.mock('../services/api', () => ({
  default: {
    get: vi.fn(() => Promise.resolve({
      data: [
        { id: 1, name: 'Sakthivelu S', dob: '2003-10-08', designation: '12th', email: 'svelu107@gmail.com' },
        { id: 2, name: 'Jane Doe', dob: '2001-05-15', designation: 'Developer', email: 'jane@example.com' }
      ]
    })),
    post: vi.fn(() => Promise.resolve({ data: {} })),
    delete: vi.fn(() => Promise.resolve({ data: {} }))
  }
}));

describe('Students Directory Dashboard Tests', () => {
  beforeEach(() => {
    localStorage.clear();
    vi.clearAllMocks();
  });

  it('should hide Write/Create controls and render student list when role is Student', async () => {
    localStorage.setItem('role', 'Student');
    render(<Students />);

    await waitFor(() => {
      expect(screen.getByText('Sakthivelu S')).toBeInTheDocument();
    });

    expect(screen.queryByText(/Add New Student Record/i)).not.toBeInTheDocument();
  });

  it('should show Write/Create controls, support adding students, and support deletion when role is Teacher', async () => {
    localStorage.setItem('role', 'Teacher');
    const { container } = render(<Students />);

    await waitFor(() => {
      expect(screen.getByText('Add New Student Record (Teacher Only)')).toBeInTheDocument();
    });

    // Test Adding a student (covers handleCreateChange and handleCreateSubmit)
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

    // Test Deleting a student (covers handleDelete)
    const deleteButtons = screen.getAllByRole('button', { name: /delete/i });
    fireEvent.click(deleteButtons[0]);

    await waitFor(() => {
      expect(api.delete).toHaveBeenCalled();
    });
  });

  it('should filter student rows live as the user types in the search field', async () => {
    localStorage.setItem('role', 'Student');
    render(<Students />);

    await waitFor(() => {
      expect(screen.getByText('Sakthivelu S')).toBeInTheDocument();
    });

    const searchInput = screen.getByPlaceholderText(/search students/i);
    fireEvent.change(searchInput, { target: { value: 'Sakthivelu' } });
    
    expect(screen.getByText('Sakthivelu S')).toBeInTheDocument();
    expect(screen.queryByText('Jane Doe')).not.toBeInTheDocument();
  });
});
