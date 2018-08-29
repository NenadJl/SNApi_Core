using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SN_App.Repo.Helpers;
using SN_App.Repo.Models;

namespace SN_App.Repo.Data.Repositories.Users
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataDBContext _context;
        public DatingRepository(DataDBContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            return await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos)
                            .AsQueryable()
                            .Where(u => u.Id != userParams.UserId)
                            .Where(u => u.Gender == userParams.Gender);

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDoB = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDoB = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDoB && u.DateOfBirth <= maxDoB);
            }

            if (userParams.OrderBy != null)
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderBy(u => u.Created);
                        break; 
                    default:
                        users = users.OrderBy(u => u.LastActive); 
                        break;
                }
            }
                
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}