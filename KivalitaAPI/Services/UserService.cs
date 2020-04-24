using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using System;
using System.Security.Cryptography;
using System.Text;

namespace KivalitaAPI.Services
{

    public class UserService : Service<User, KivalitaApiContext, UserRepository>
    {
        public UserService(KivalitaApiContext context, UserRepository baseRepository) : base(context, baseRepository) { }

        public override User Add(User user)
        {
            user.Password = Encrypt(user.Password);
            return base.Add(user);
        }

        public User GetByLoginData(User user)
        {
            user.Password = Encrypt(user.Password);
            return this.baseRepository.GetByLoginData(user);
        }

        public override User Update(User user)
        {
            user.Password = Encrypt(user.Password);
            return base.Update(user);
        }

        private string Encrypt(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            string hash = String.Empty;
            byte[] crypto = sha256.ComputeHash(Encoding.ASCII.GetBytes(str));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }

    }
}


