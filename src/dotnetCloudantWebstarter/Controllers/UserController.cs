using CloudantDotNet.Models;
using CloudantDotNet.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudantDotNet.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserCloudantService _cloudantService;

        public UserController(IUserCloudantService cloudantService)
        {
            _cloudantService = cloudantService;
        }

        // GET: api/values
        [HttpGet]
        [Route("api/user")]
        public async Task<dynamic> Get(string userMail)
        {
            return await _cloudantService.GetUserAsync(userMail);
        }

        // PUT api/user
        [HttpPut]
        [Route("api/user/SaveToken")]
        public async Task<dynamic> PutToken([FromBody]UserRequestDto userReq)
        {
            if (userReq.recent_token == null || string.IsNullOrWhiteSpace(userReq.recent_token))
                return null;

            userReq.prev_token = (string.IsNullOrWhiteSpace(userReq.prev_token)) ? "" : userReq.prev_token;
            userReq.recent_token = (string.IsNullOrWhiteSpace(userReq.recent_token)) ? "" : userReq.recent_token;
            userReq.user_mail = (string.IsNullOrWhiteSpace(userReq.user_mail)) ? "" : userReq.user_mail;
            userReq.prev_user_mail = (string.IsNullOrWhiteSpace(userReq.prev_user_mail)) ? "" : userReq.prev_user_mail;

            try
            {
                List<User> userList = await _cloudantService.GetTokenAsync(userReq.prev_token);
                if (userList != null && userList.Any())
                {
                    foreach (var item in userList)
                    {
                        item.token = userReq.recent_token;
                        if (!string.IsNullOrWhiteSpace(userReq.user_mail))
                        {
                            item.user_mail = userReq.user_mail;
                        }
                        await _cloudantService.UpdateAsync(item);
                    }
                }
                else
                {
                    User user = new User()
                    {
                        user_mail = userReq.user_mail,
                        token = userReq.recent_token,
                        push_cekilis = userReq.push_cekilis,
                        push_win = userReq.push_win,
                        time = DateTime.Now.ToString()
                    };
                    await _cloudantService.CreateAsync(user);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        [HttpPut]
        [Route("api/user/SaveUser")]
        public async Task<dynamic> PutUser([FromBody]UserRequestDto userReq)
        {
            if (string.IsNullOrWhiteSpace(userReq.user_mail))
                return null;
            if (string.IsNullOrWhiteSpace(userReq.recent_token))
                return null;

            userReq.prev_token = (string.IsNullOrWhiteSpace(userReq.prev_token)) ? "" : userReq.prev_token;
            userReq.recent_token = (string.IsNullOrWhiteSpace(userReq.recent_token)) ? "" : userReq.recent_token;
            userReq.user_mail = (string.IsNullOrWhiteSpace(userReq.user_mail)) ? "" : userReq.user_mail;
            userReq.prev_user_mail = (string.IsNullOrWhiteSpace(userReq.prev_user_mail)) ? "" : userReq.prev_user_mail;

            try
            {
                List<User> userList = await _cloudantService.GetUserAsync(userReq.user_mail);
                if (userList != null && userList.Any())
                {
                    foreach (var user in userList)
                    {
                        if (!user.token.Equals(userReq.recent_token))
                        {
                            user.token = userReq.recent_token;
                            await _cloudantService.UpdateAsync(user);
                        }
                    }
                }
                else
                {
                    User user = new User()
                    {
                        user_mail = userReq.user_mail,
                        token = userReq.recent_token,
                        push_cekilis = userReq.push_cekilis,
                        push_win = userReq.push_win,
                        time = DateTime.Now.ToString()
                    };
                    await _cloudantService.CreateAsync(user);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}
