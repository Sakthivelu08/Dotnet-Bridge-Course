import React from 'react';

const StudentRow = ({ student, onDelete, userRole }) => {
  return (
    <tr>
      <td>{student.name}</td>
      <td>{new Date(student.dob).toLocaleDateString()}</td>
      <td>{student.designation}</td>
      <td>{student.email}</td>
      <td>
        {userRole === 'Teacher' && (
          <button className="btn-delete" onClick={() => onDelete(student.id)}>
            Delete
          </button>
        )}
      </td>
    </tr>
  );
};

export default StudentRow;
