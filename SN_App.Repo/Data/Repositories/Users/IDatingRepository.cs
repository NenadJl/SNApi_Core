using System.Collections.Generic;
using System.Threading.Tasks;
using SN_App.Repo.Models;

namespace SN_App.Repo.Data.Repositories.Users
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
    }
}