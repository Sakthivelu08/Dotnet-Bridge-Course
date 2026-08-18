namespace BridgeCourse.Week4.Api.Data.EfDbFirst.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordSalt { get; set; } = new byte[0];
        public string Role { get; set; } = string.Empty;
    }
}
