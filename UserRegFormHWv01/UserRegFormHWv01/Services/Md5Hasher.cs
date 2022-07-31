namespace UserRegFormHWv01.Services
{
    public class Md5Hasher : IHasher
    {
        public string Hash(string str)
        {
            using var algo = System.Security.Cryptography.MD5.Create();
            byte[] hash = algo.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
            var sb = new System.Text.StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X02"));
            }
            return sb.ToString();
        }

        public string Hash(string str, string hashSalt)
        {
            return Hash(str + hashSalt);
        }

        public string UnHash(string str, string hashSalt)
        {
            return Hash(str + hashSalt);
        }
    }
}
