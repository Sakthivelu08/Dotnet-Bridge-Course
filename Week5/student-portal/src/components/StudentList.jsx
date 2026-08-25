import React from 'react';
import StudentRow from './StudentRow';

const StudentList = ({ students, onDelete, userRole, isLoading }) => {
  if (isLoading) {
    return <p>Loading student records...</p>;
  }

  if (students.length === 0) {
    return <p>No students found.</p>;
  }

  return (
    <table className="student-table">
      <thead>
        <tr>
          <th>Name</th>
          <th>DOB</th>
          <th>Designation</th>
          <th>Email</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        {students.map((student) => (
          <StudentRow 
            key={student.id} 
            student={student} 
            onDelete={onDelete} 
            userRole={userRole} 
          />
        ))}
      </tbody>
    </table>
  );
};

export default StudentList;
