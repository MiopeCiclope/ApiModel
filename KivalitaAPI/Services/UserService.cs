using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KivalitaAPI.Services
{

    public class UserService : Service<User, KivalitaApiContext, UserRepository>
    {
        CompanyRepository companyRepository;

        public UserService(KivalitaApiContext context, UserRepository baseRepository, CompanyRepository companyRepository) : base(context, baseRepository)
        {
            this.companyRepository = companyRepository;
        }

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
            var oldUser = this.baseRepository.Get(user.Id);


            var companyToUnlink = oldUser.Company.Except(user.Company);
            var companyToLink = user.Company.Except(oldUser.Company);


            foreach (var c in companyToUnlink)
            {
                c.UserId = null;
                companyRepository.Update(c);
            }

            foreach (var c in companyToLink)
            {
                c.UserId = user.Id;
                companyRepository.Update(c);
            }

            if (!String.IsNullOrEmpty(user.Password)) 
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


