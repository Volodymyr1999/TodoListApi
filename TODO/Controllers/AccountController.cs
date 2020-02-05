using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TODO;
using TODO.Services;
using TODO.ViewModels;

namespace TODO.Controllers
{
    /// <summary>
    /// accounts
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    
    public class AccountController : ControllerBase
    {
        Models.TodoContext db;
        private readonly AuthenticationService authService;
        private readonly UserService userService;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="todoContext"></param>
        /// <param name="service"></param>
        /// <param name="uservice"></param>
        public AccountController(Models.TodoContext todoContext,AuthenticationService service,UserService uservice)
        {
            db = todoContext;
            authService = service;
            userService = uservice;
        }

        /// <summary>
        /// login existed user
        /// </summary>
        /// <param name="userView"></param>
        /// <returns>username and token</returns>
        [HttpPost("login/")]
        public async Task Token([FromForm]UserView userView) //login
        {
            var user = await userService.FindUser(userView);
            if (user != null)
            {
                var encodedjwt = authService.CreateToken(user); //create and encode created jwt
                var response = new
                {
                    access_token = encodedjwt,
                    userName = userView.UserName
                };
                Response.ContentType = "application/json";
                await Response.WriteAsync(JsonConvert.SerializeObject(response, Formatting.Indented));
                return;
            }
            await Response.WriteAsync("wrong username or password");
        }
        /// <summary>
        /// register new user and add it to database
        /// </summary>
        /// <param name="userView"></param>
        /// <returns></returns>
        [HttpPost("register/")]

        public async Task Register([FromForm]UserView userView) //register
        {
            Models.User user=userService.CreateUser(userView);
            var encodedjwt = authService.CreateToken(user);

            var response = new
            {
                access_token = encodedjwt,
                username = userView.UserName
            };
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
    }
}