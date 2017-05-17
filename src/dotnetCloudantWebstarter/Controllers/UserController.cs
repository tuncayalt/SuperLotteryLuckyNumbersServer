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
                        item.push_cekilis = userReq.push_cekilis.Length > 1 ? userReq.push_cekilis.Substring(1, 1) : "T";
                        item.push_win = userReq.push_win.Length > 1 ? userReq.push_win.Substring(1, 1) : "T";
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
                        push_cekilis = userReq.push_cekilis.Length > 1 ? userReq.push_cekilis.Substring(1,1) : "T",
                        push_win = userReq.push_win.Length > 1 ? userReq.push_win.Substring(1, 1) : "T",
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
                        if (!user.token.Equals(userReq.recent_token) || (userReq.push_cekilis.Length > 1 && userReq.push_cekilis.Substring(0,1).Equals("D")))
                        {
                            user.token = userReq.recent_token;
                            user.push_cekilis = userReq.push_cekilis.Substring(1,1);
                            user.push_win = userReq.push_win.Substring(1, 1);
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
                        push_cekilis = userReq.push_cekilis.Substring(1,1),
                        push_win = userReq.push_win.Substring(1, 1),
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
