using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Data.AdoNet
{
    public class AdoNetUserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public AdoNetUserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("DefaultConnection connection string is not configured.");
        }

        public User? GetByUsername(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT Username, PasswordHash, PasswordSalt, Role FROM Users WHERE Username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Username = reader.GetString(0),
                                PasswordHash = (byte[])reader.GetValue(1),
                                PasswordSalt = (byte[])reader.GetValue(2),
                                Role = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Add(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(
                    "INSERT INTO Users (Username, PasswordHash, PasswordSalt, Role) VALUES (@Username, @PasswordHash, @PasswordSalt, @Role)", 
                    connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
                    command.Parameters.AddWithValue("@Role", user.Role);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
