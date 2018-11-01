namespace DatingApp.API.Utils.Security
{
    public static class SaltHashing
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                passwordSalt = hmac.Key;
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
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