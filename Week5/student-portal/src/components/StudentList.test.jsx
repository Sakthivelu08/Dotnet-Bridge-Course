import React from 'react';
import { render, screen } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import StudentList from './StudentList';

describe('StudentList Component Tests', () => {
  const mockStudents = [
    { id: 1, name: 'Alice Smith', dob: '2001-05-15', designation: 'Developer', email: 'alice@example.com' },
    { id: 2, name: 'Bob Jones', dob: '1999-10-22', designation: 'QA Engineer', email: 'bob@example.com' }
  ];

  it('should render loading indicator when isLoading is true', () => {
    render(<StudentList students={[]} onDelete={vi.fn()} userRole="Student" isLoading={true} />);
    expect(screen.getByText('Loading student records...')).toBeInTheDocument();
  });

  it('should render empty state when no students are present', () => {
    render(<StudentList students={[]} onDelete={vi.fn()} userRole="Student" isLoading={false} />);
    expect(screen.getByText('No students found.')).toBeInTheDocument();
  });

  it('should render student table rows from props', () => {
    render(<StudentList students={mockStudents} onDelete={vi.fn()} userRole="Student" isLoading={false} />);
    
    expect(screen.getByText('Alice Smith')).toBeInTheDocument();
    expect(screen.getByText('Bob Jones')).toBeInTheDocument();
    expect(screen.getByText('alice@example.com')).toBeInTheDocument();
  });
});
