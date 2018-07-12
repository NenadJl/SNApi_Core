using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SN_App.Repo.Models;

namespace SN_App.Repo.Data
{
    public class Seed
    {
        private DataDBContext _context;
        public Seed(DataDBContext context)
        {
            _context = context;
        }

        public void SeedUsers()
        {
            var userData = System.IO.File.ReadAllText("D:/Training_Projects/SN_App/SN_App.Repo/Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            
            foreach (var user in users)
            {
                byte[] passwordHash, passwordSalt;

                CreatePasswordHash("password", out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();

                _context.Users.Add(user);
            }

            _context.SaveChanges();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            };
        }
    }
}