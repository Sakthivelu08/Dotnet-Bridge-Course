using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Data.AdoNet
{
    public class AdoNetStudentRepository : IRepository<Student>
    {
        private readonly string _connectionString;

        public AdoNetStudentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("DefaultConnection connection string is not configured.");
        }

        public IEnumerable<Student> GetAll()
        {
            var students = new List<Student>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT Id, Name, Age, Email, Grade, InternalNotes, EnrolledOn FROM Students", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(MapReaderToStudent(reader));
                        }
                    }
                }
            }
            return students;
        }

        public Student? GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("usp_GetStudentById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToStudent(reader);
                        }
                    }
                }
            }
            return null;
        }

        public void Add(Student entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("usp_InsertStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Age", entity.Age);
                    command.Parameters.AddWithValue("@Email", entity.Email);
                    command.Parameters.AddWithValue("@Grade", entity.Grade);
                    command.Parameters.AddWithValue("@InternalNotes", (object?)entity.InternalNotes ?? DBNull.Value);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        entity.Id = Convert.ToInt32(result);
                    }
                }
            }
        }

        public void Update(Student entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(
                    "UPDATE Students SET Name = @Name, Age = @Age, Email = @Email, Grade = @Grade, InternalNotes = @InternalNotes WHERE Id = @Id", 
                    connection))
                {
                    command.Parameters.AddWithValue("@Id", entity.Id);
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Age", entity.Age);
                    command.Parameters.AddWithValue("@Email", entity.Email);
                    command.Parameters.AddWithValue("@Grade", entity.Grade);
                    command.Parameters.AddWithValue("@InternalNotes", (object?)entity.InternalNotes ?? DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("DELETE FROM Students WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private Student MapReaderToStudent(SqlDataReader reader)
        {
            return new Student
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Age = reader.GetInt32(2),
                Email = reader.GetString(3),
                Grade = reader.GetDecimal(4),
                InternalNotes = reader.IsDBNull(5) ? null : reader.GetString(5),
                EnrolledOn = reader.GetDateTime(6)
            };
        }
    }
}
