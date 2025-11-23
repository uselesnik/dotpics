using DotPic.Models;

namespace DotPic.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserAsync(string id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(string id, User user);
        Task DeleteUserAsync(string id);
        Task AddTestUserAsync(); // Add this line
         Task UpdateUserImageAsync(string id, byte[] imageData, string contentType, string fileName); 
    }
}