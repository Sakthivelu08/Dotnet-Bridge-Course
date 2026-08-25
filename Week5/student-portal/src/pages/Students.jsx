import React, { useState, useEffect } from 'react';
import api from '../services/api';
import StudentList from '../components/StudentList';

const Students = () => {
  const [students, setStudents] = useState([]);
  const [search, setSearch] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [userRole, setUserRole] = useState('');
  const [error, setError] = useState('');
  
  const [newStudent, setNewStudent] = useState({
    name: '',
    dob: '',
    designation: '',
    email: ''
  });

  const fetchStudents = async () => {
    try {
      const response = await api.get('/students');
      setStudents(response.data);
    } catch (err) {
      setError('Could not load student records from the database.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    const role = localStorage.getItem('role');
    setUserRole(role);
    fetchStudents();
  }, []);

  const handleDelete = async (id) => {
    try {
      await api.delete(`/students/${id}`);
      setStudents(students.filter(s => s.id !== id));
    } catch (err) {
      alert('Delete operation failed.');
    }
  };

  const handleCreateSubmit = async (e) => {
    e.preventDefault();
    try {
      await api.post('/students', newStudent);
      setNewStudent({ name: '', dob: '', designation: '', email: '' });
      fetchStudents();
    } catch (err) {
      alert('Could not add new student.');
    }
  };

  const handleCreateChange = (e) => {
    setNewStudent({
      ...newStudent,
      [e.target.name]: e.target.value
    });
  };

  const filteredStudents = students.filter(s =>
    s.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="dashboard-container">
      <h2>Student Directory (Role: {userRole})</h2>
      {error && <div className="error-banner">{error}</div>}
      
      <div className="search-section">
        <input 
          type="text" 
          placeholder="Search students by name..." 
          value={search} 
          onChange={e => setSearch(e.target.value)} 
          className="search-input"
        />
        <span>Matches found: {filteredStudents.length}</span>
      </div>

      {userRole === 'Teacher' && (
        <div style={{ border: '1px solid var(--theme-border)', padding: '20px', borderRadius: '6px', marginBottom: '25px', backgroundColor: 'var(--theme-surface)' }}>
          <h3>Add New Student Record (Teacher Only)</h3>
          <form onSubmit={handleCreateSubmit} style={{ display: 'flex', gap: '15px', flexWrap: 'wrap' }}>
            <input name="name" placeholder="Name" onChange={handleCreateChange} value={newStudent.name} required className="search-input" style={{ width: '180px' }} />
            <input type="date" name="dob" onChange={handleCreateChange} value={newStudent.dob} required className="search-input" style={{ width: '150px' }} />
            <input name="designation" placeholder="Designation" onChange={handleCreateChange} value={newStudent.designation} required className="search-input" style={{ width: '150px' }} />
            <input type="email" name="email" placeholder="Email" onChange={handleCreateChange} value={newStudent.email} required className="search-input" style={{ width: '180px' }} />
            <button type="submit" className="btn-primary" style={{ width: 'auto', padding: '10px 25px' }}>Add</button>
          </form>
        </div>
      )}

      <StudentList 
        students={filteredStudents} 
        onDelete={handleDelete} 
        userRole={userRole} 
        isLoading={isLoading} 
      />
    </div>
  );
};

export default Students;
