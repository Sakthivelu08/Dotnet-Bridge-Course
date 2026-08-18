using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Data.AdoNet
{
    public class AdoNetTeacherRepository : IRepository<Teacher>
    {
        private readonly string _connectionString;

        public AdoNetTeacherRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("DefaultConnection connection string is not configured.");
        }

        public IEnumerable<Teacher> GetAll()
        {
            var teachers = new List<Teacher>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT Id, Name, Email, Subject, Salary, InternalNotes FROM Teachers", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            teachers.Add(MapReaderToTeacher(reader));
                        }
                    }
                }
            }
            return teachers;
        }

        public Teacher? GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT Id, Name, Email, Subject, Salary, InternalNotes FROM Teachers WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToTeacher(reader);
                        }
                    }
                }
            }
            return null;
        }

        public void Add(Teacher entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(
                    "INSERT INTO Teachers (Name, Email, Subject, Salary, InternalNotes) VALUES (@Name, @Email, @Subject, @Salary, @InternalNotes); SELECT SCOPE_IDENTITY();", 
                    connection))
                {
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Email", entity.Email);
                    command.Parameters.AddWithValue("@Subject", entity.Subject);
                    command.Parameters.AddWithValue("@Salary", entity.Salary);
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

        public void Update(Teacher entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(
                    "UPDATE Teachers SET Name = @Name, Email = @Email, Subject = @Subject, Salary = @Salary, InternalNotes = @InternalNotes WHERE Id = @Id", 
                    connection))
                {
                    command.Parameters.AddWithValue("@Id", entity.Id);
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Email", entity.Email);
                    command.Parameters.AddWithValue("@Subject", entity.Subject);
                    command.Parameters.AddWithValue("@Salary", entity.Salary);
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
                using (var command = new SqlCommand("DELETE FROM Teachers WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private Teacher MapReaderToTeacher(SqlDataReader reader)
        {
            return new Teacher
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Subject = reader.GetString(3),
                Salary = reader.GetDecimal(4),
                InternalNotes = reader.IsDBNull(5) ? null : reader.GetString(5)
            };
        }
    }
}
