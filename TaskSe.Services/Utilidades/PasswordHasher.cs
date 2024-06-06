using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Math;

namespace TaskSe.Services.Utilidades
{
    public class PasswordHasher
    {
        private const int WorkFactor = 10; // Factor de trabajo (costo de hashing)
        private const string Pepper = "jekabc%*."; // Pepper secreto

        // Hash de una contraseña con salt y pepper
        public static string HashPassword(string password)
        {
            string saltedPassword = password + Pepper;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(saltedPassword, WorkFactor);
            return hashedPassword;
        }

        // Verificación de una contraseña
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string saltedPassword = password + Pepper;
            bool verified = BCrypt.Net.BCrypt.Verify(saltedPassword, hashedPassword);
            return verified;
        }
    }
}
