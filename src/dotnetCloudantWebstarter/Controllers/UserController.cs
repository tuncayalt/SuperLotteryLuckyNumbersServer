using CloudantDotNet.Models;
using CloudantDotNet.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserCloudantService _cloudantService;

        public UserController(IUserCloudantService cloudantService)
        {
            _cloudantService = cloudantService;
        }

        // GET: api/values
        [HttpGet]
        public async Task<dynamic> Get(string userMail)
        {
            return await _cloudantService.GetUserAsync(userMail);
        }

        // PUT api/values
        [HttpPut]
        public async Task<dynamic> Put(string prev_token, string recent_token, string user_mail)
        {
            List<User> userList = await _cloudantService.GetTokenAsync(prev_token);
            if (userList != null && userList.Any())
            {
                foreach (var item in userList)
                {
                    item.token = recent_token;
                    if (!string.IsNullOrWhiteSpace(user_mail))
                    {
                        item.user_mail = user_mail;
                    }
                    await _cloudantService.UpdateAsync(item);
                }
            }
            else
            {
                User user = new User()
                {
                    user_mail = user_mail,
                    token = recent_token,
                    push_cekilis = true,
                    push_win = true
                };
                await _cloudantService.UpdateAsync(user);
            }


            return userList;
        }
    }
}
