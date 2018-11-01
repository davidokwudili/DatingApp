using System;
using System.Threading.Tasks;
using DatingApp.API.Utils.Security;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Datas
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _db;
        public AuthRepository(DataContext db)
        {
            _db = db;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
                return null;

            // if (!SaltHashing.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            //     return null;

            return user;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            //the out keyword will set value back into the variables
            SaltHashing.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //set user password values
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            //save user async
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            //return user
            return user;
        }

        public async Task<bool> UserExist(string username)
        {
            if (await _db.Users.AnyAsync(x => x.Username == username))
                return true;
            else
                return false;
        }

    }
}