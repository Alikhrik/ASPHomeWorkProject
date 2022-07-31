namespace UserRegFormHWv01.Services
{
    public interface IHasher
    {
        public string Hash(string str);

        public string Hash(string str, string hashSalt);
        public string UnHash(string str, string hashSalt);
    }
}
