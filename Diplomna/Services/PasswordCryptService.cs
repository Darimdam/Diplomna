using System.Security.Cryptography;

namespace Diplomna.Services
{
    public static class PasswordCryptService
    {
        public static string CryptPassword(string inputString) // При проверка не се декриптира, а се криптира подаденото и се сверява с вече криптираната парола
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(inputString);
            data = new SHA256Managed().ComputeHash(data);
            string hash = System.Text.Encoding.ASCII.GetString(data);
            return hash;
        }
    }
}
