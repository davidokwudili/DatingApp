using System.Threading.Tasks;
using DatingApp.Models;

namespace DatingApp.Datas
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExist(string username);

    }
}