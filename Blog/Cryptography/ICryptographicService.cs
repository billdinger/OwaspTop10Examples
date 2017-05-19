namespace Blog.Cryptography
{
    public interface ICryptographicService
    {
        string HashPassword(string password, string salt);
    }
}