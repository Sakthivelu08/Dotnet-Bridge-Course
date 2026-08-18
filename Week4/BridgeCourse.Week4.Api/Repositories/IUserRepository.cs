using BridgeCourse.Week4.Api.Models;

namespace BridgeCourse.Week4.Api.Repositories
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        void Add(User user);
    }
}
