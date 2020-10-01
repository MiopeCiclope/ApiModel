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
        MicrosoftTokenRepository microsoftTokenRepository;
        MailSignatureService mailSignatureService;
        MailSignatureRepository mailSignatureRepository;

        public UserService(
            KivalitaApiContext context,
            UserRepository baseRepository,
            CompanyRepository companyRepository,
            MicrosoftTokenRepository microsoftTokenRepository,
            MailSignatureService _mailSignatureService,
            MailSignatureRepository _mailSignatureRepository
        ) : base(context, baseRepository)
        {
            this.companyRepository = companyRepository;
            this.microsoftTokenRepository = microsoftTokenRepository;
            this.mailSignatureService = _mailSignatureService;
            this.mailSignatureRepository = _mailSignatureRepository;
        }

        public override User Get(int id)
        {
            var user = baseRepository.Get(id);

            var signature = mailSignatureRepository.GetBy(signature => signature.UserId == user.Id)?.First() ?? new MailSignature { Id = 0, Signature = "" };
            user.MailSignature = signature;

            var microsoftToken = this.microsoftTokenRepository.GetBy(m => m.UserId == user.Id)
                .FirstOrDefault();
            user.LinkedMicrosoftGraph = microsoftToken != null ? true : false;
            return user;
        }

        public override User Add(User user)
        {
            user.Password = Encrypt("kivalita@2020");
            return base.Add(user);
        }

        public User GetByLoginData(User user)
        {
            user.Password = Encrypt(user.Password);
            return this.baseRepository.GetByLoginData(user);
        }

        public override User Update(User user)
        {
            try
            {
                var oldUser = this.baseRepository.Get(user.Id);
                if(user.Company != null)
                {
                    var companyToUnlink = oldUser.Company.Where(company => !user.Company.Select(company => company.Id)?.Contains(company.Id) ?? true);
                    var companyToLink = user.Company.Where(company => !oldUser.Company?.Select(companyUnlink => companyUnlink.Id).Contains(company.Id) ?? true);
                    if(companyToUnlink.Any()) {
                        var companyList = companyToUnlink.ToList();
                        companyList.ForEach(company => company.UserId = null);
                        companyRepository.UpdateRange(companyList);
                    }

                    if (companyToLink.Any())
                    {
                        var companyList = companyToLink.ToList();
                        companyList.ForEach(company => company.UserId = user.Id);
                        companyRepository.UpdateRange(companyList);
                    }
                }

                if (user.MailSignature != null)
                {
                    var signature = mailSignatureRepository.GetBy(signature => signature.UserId == user.Id)?.First() ?? null;
                    if (signature == null)
                        user.MailSignatureId = mailSignatureService.Add(user.MailSignature).Id;
                    else
                    {
                        signature.Signature = user.MailSignature.Signature;
                        mailSignatureService.Update(signature);
                    }
                }

                if (!String.IsNullOrEmpty(user.Password)) 
                    user.Password = Encrypt(user.Password);

                return base.Update(user);
            }
            catch(Exception e)
            {
                return null;
            }
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

        public string GetSignature(int id)
        {
            var mailSignature = mailSignatureRepository.GetBy(signature => signature.UserId == id).FirstOrDefault();
            return mailSignature != null ? mailSignature.Signature : "";
        }
    }
}


