using System.Threading.Tasks;
using SN_App.Repo.Models;

namespace SN_App.Repo.Data.Repositories.Authentication
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string password);
         Task<bool> UserExists(string username);
         Task<User> Login(string username, string password);
    }
}