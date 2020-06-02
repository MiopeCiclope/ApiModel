
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using System;
using KivalitaAPI.Common;
using System.Net;
using KivalitaAPI.DTOs;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController :  CustomController<User, UserService>
    {
        private readonly UserService service;
        private readonly ILogger logger;

        public UserController(UserService service, ILogger<UserController> logger) : base(service, logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [HttpPost]
        [Route("auth")]
        [AllowAnonymous]
        public HttpResponse<Token> Authenticate([FromBody] AuthDTO userCredentials)
        {
            try
            {
                var tokenService = new TokenService(this.service.context, new Repositories.TokenRepository(this.service.context));
                if (userCredentials.GrantType == "password")
                {
                    var fakeUser = new User
                    {
                        Email = userCredentials.Email,
                        Password = userCredentials.Password
                    };

                    var user = this.service.GetByLoginData(fakeUser);
                    if (user == null)
                    {
                        return new HttpResponse<Token>
                        {
                            IsStatusCodeSuccess = false,
                            data = null,
                            statusCode = HttpStatusCode.NotFound,
                            ErrorMessage = "Usuário ou senha inválidos"
                        };
                    }

                    var token = tokenService.GenerateToken(user);
                    token.User.Password = "";
                    token.Id = 0;

                    return new HttpResponse<Token>
                    {
                        IsStatusCodeSuccess = true,
                        data = token,
                        statusCode = HttpStatusCode.OK
                    };
                }
                else if(userCredentials.GrantType == "refresh_token")
                {
                    if(String.IsNullOrEmpty(userCredentials.RefreshToken))
                    {
                        return new HttpResponse<Token>
                        {
                            IsStatusCodeSuccess = false,
                            data = null,
                            statusCode = HttpStatusCode.BadRequest,
                            ErrorMessage = "O RefreshToken é obrigatório para o GratType 'refresh_token'"
                        };
                    }
                    else
                    {
                        var token = tokenService.RefreshToken(userCredentials.RefreshToken);
                        token.User.Password = "";
                        token.Id = 0;

                        return new HttpResponse<Token>
                        {
                            IsStatusCodeSuccess = true,
                            data = token,
                            statusCode = HttpStatusCode.OK
                        };  
                    }
                }
                else
                {
                    return new HttpResponse<Token>
                    {
                        IsStatusCodeSuccess = false,
                        data = null,
                        statusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = "GrantType inválido"
                    };
                }
            }
            catch(Exception e)
            {
                this.logger.LogError(e.Message);
                return new HttpResponse<Token>
                {
                    IsStatusCodeSuccess = false,
                    data = null,
                    statusCode = HttpStatusCode.InternalServerError,
                    ErrorMessage = "Erro ao realizar a autenticaçâo"
                };
            }
        }
    }
}
