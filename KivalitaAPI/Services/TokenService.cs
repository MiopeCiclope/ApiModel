
using KivalitaAPI.Data;
using KivalitaAPI.Enum;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace KivalitaAPI.Services
{

    public class TokenService : Service<Token, KivalitaApiContext, TokenRepository>
    {
        private UserRepository _userRepository;
        public TokenService(KivalitaApiContext context, TokenRepository baseRepository) : base(context, baseRepository) 
        {
            this._userRepository = new UserRepository(this.context);
        }

        public Token GenerateToken(User user, LoginTypeEnum Client)
        {
            var authToken = createToken(user);
            authToken.LoginClient = Client;

            var tokenSearch = this.baseRepository
                                    .GetBy(storedToken => storedToken.UserId == authToken.UserId && storedToken.LoginClient == Client);

            if(tokenSearch.Any())
            {
                authToken.Id = tokenSearch.First().Id;
                this.baseRepository.Update(authToken);
            }
            else 
                this.baseRepository.Add(authToken);

            authToken.User = user;
            return authToken;
        }
        public Token RefreshToken(string refreshToken, LoginTypeEnum Client)
        {
            var expiredToken = this.baseRepository
                                    .GetBy(storedToken => storedToken.RefreshToken == refreshToken && storedToken.LoginClient == Client);

            if (!expiredToken.Any())
                return null;
            else
            {
                var oldToken = expiredToken.First();
                var user = this._userRepository.Get(oldToken.UserId);

                var newToken = createToken(user);
                newToken.Id = oldToken.Id;
                newToken.LoginClient = Client;

                this.baseRepository.Update(newToken);
                newToken.User = user;
                return newToken;
            }
        }

        private Token createToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Setting.Secret);
            var expiration = DateTime.UtcNow.AddHours(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Role", user.Role)
                }),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Token
            {
                AccessToken = tokenHandler.WriteToken(token),
                ExpirationDate = expiration,
                RefreshToken = Guid.NewGuid().ToString().Replace("-", String.Empty),
                UserId = user.Id
            };
        }
    }
}


