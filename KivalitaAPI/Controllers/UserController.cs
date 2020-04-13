
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using System;
using System.Threading.Tasks;
using KivalitaAPI.Common;
using System.Collections.Generic;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomController<User, UserService>
    {
        public UserController(UserService service, ILogger<UserController> logger) : base(service, logger) { }

        [HttpPost]
        [Route("auth")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User userCredentials)
        {
            try
            {
                var user = this.service.GetByLoginData(userCredentials);
                if (user == null)
                    return NotFound(new { message = "Usuário ou senha inválidos" });

                var token = TokenService.GenerateToken(user);
                user.Password = "";
                return new
                {
                    user = user,
                    token = token
                };
            }
            catch(Exception e)
            {
                return NotFound(new { message = "Usuário ou senha inválidos" });
            }
        }

        //public override HttpResponse<List<User>> Get()
        //{
        //    return base.Get();
        //}

        //public override HttpResponse<User> Get(int id)
        //{
        //    return base.Get(id);
        //}

        //public override HttpResponse<User> Post(User entity)
        //{
        //    return base.Post(entity);
        //}
        //public override HttpResponse<User> Put(int id, User entity)
        //{
        //    return base.Put(id, entity);
        //}
        //public override HttpResponse<User> Delete(int id)
        //{
        //    return base.Delete(id);
        //}
    }
}
