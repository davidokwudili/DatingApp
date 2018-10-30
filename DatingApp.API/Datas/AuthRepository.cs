using System;
using System.Threading.Tasks;
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

            // if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            //     return null;

            return user;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            //the out keyword will set value back into the variables
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

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



        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                passwordSalt = hmac.Key;
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // pass the passwordSalt as the key
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (var i = 0; i < computedHash.Length; i++)
                {
                    //check if passwordHashed not equal the typed in password
                    if (passwordHash[i] != computedHash[i])
                        return false;
                }
                return true;
            }
        }
    }
}