using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Utils.Security;
using DatingApp.Datas;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {

        private readonly DataContext _context;

        public Seed(DataContext context)
        {
            this._context = context;
        }


        public void SeedUsers()
        {
            var userData = System.IO.File.ReadAllText("Datas/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            foreach (var user in users)
            {
                byte[] passwordHash, passwordSalt;
                SaltHashing.CreatePasswordHash("password", out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();

                _context.Users.Add(user);
            }

            _context.SaveChanges();
        }

    }

}