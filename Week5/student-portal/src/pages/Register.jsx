import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const Register = () => {
  const navigate = useNavigate();
  
  const [formData, setFormData] = useState({
    name: '',
    dob: '',
    designation: '',
    email: '',
    password: '',
    role: 0, // Maps to UserRole.Student (enum value 0)
  });

  const [error, setError] = useState('');

  const isFormValid = 
    formData.name.length >= 3 && 
    formData.email.includes('@') && 
    formData.password.length >= 6 &&
    formData.dob !== '' &&
    formData.designation !== '';

  const handleChange = (e) => {
    const value = e.target.name === 'role' ? parseInt(e.target.value) : e.target.value;
    setFormData({
      ...formData,
      [e.target.name]: value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await api.post('/auth/register', formData);
      navigate('/login');
    } catch (err) {
      setError(err.response?.data?.error || 'Registration failed.');
    }
  };

  return (
    <div className="auth-container">
      <h2>Register</h2>
      {error && <div className="error-banner">{error}</div>}
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Full Name</label>
          <input 
            name="name" 
            className="form-input" 
            onChange={handleChange} 
            value={formData.name} 
            required 
          />
        </div>
        <div className="form-group">
          <label>Date of Birth</label>
          <input 
            type="date" 
            name="dob" 
            className="form-input" 
            onChange={handleChange} 
            value={formData.dob} 
            required 
          />
        </div>
        <div className="form-group">
          <label>Designation</label>
          <input 
            name="designation" 
            className="form-input" 
            onChange={handleChange} 
            value={formData.designation} 
            required 
          />
        </div>
        <div className="form-group">
          <label>Email Address</label>
          <input 
            type="email" 
            name="email" 
            className="form-input" 
            onChange={handleChange} 
            value={formData.email} 
            required 
          />
        </div>
        <div className="form-group">
          <label>Password (min 6 characters)</label>
          <input 
            type="password" 
            name="password" 
            className="form-input" 
            onChange={handleChange} 
            value={formData.password} 
            required 
          />
        </div>
        <div className="form-group">
          <label>System Role</label>
          <select 
            name="role" 
            className="form-input" 
            onChange={handleChange} 
            value={formData.role} 
            required
          >
            <option value={0}>Student</option>
            <option value={1}>Teacher</option>
          </select>
        </div>

        <button type="submit" className="btn-primary" disabled={!isFormValid}>Register</button>
      </form>
    </div>
  );
};

export default Register;
