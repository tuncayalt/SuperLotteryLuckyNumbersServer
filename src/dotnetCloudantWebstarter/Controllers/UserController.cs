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
        public async Task<dynamic> PutToken(string prev_token, string recent_token, string user_mail, string prev_user_mail)
        {
            if (recent_token == null || string.IsNullOrWhiteSpace(recent_token))
                return null;

            prev_token = (string.IsNullOrWhiteSpace(prev_token)) ? "" : prev_token;
            recent_token = (string.IsNullOrWhiteSpace(recent_token)) ? "" : recent_token;
            user_mail = (string.IsNullOrWhiteSpace(user_mail)) ? "" : user_mail;
            prev_user_mail = (string.IsNullOrWhiteSpace(prev_user_mail)) ? "" : prev_user_mail;

            try
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
                        return await _cloudantService.UpdateAsync(item);
                    }
                }
                else
                {
                    User user = new User()
                    {
                        user_mail = user_mail,
                        token = recent_token,
                        push_cekilis = "T",
                        push_win = "T",
                        time = DateTime.Now.ToString()
                    };
                    return await _cloudantService.CreateAsync(user);
                }
                return userList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        [HttpPut]
        [Route("api/user/SaveUser")]
        public async Task<dynamic> PutUser(string prev_token, string recent_token, string user_mail, string prev_user_mail)
        {
            if (string.IsNullOrWhiteSpace(user_mail))
                return null;
            if (string.IsNullOrWhiteSpace(recent_token))
                return null;

            prev_token = (string.IsNullOrWhiteSpace(prev_token)) ? "" : prev_token;
            recent_token = (string.IsNullOrWhiteSpace(recent_token)) ? "" : recent_token;
            user_mail = (string.IsNullOrWhiteSpace(user_mail)) ? "" : user_mail;
            prev_user_mail = (string.IsNullOrWhiteSpace(prev_user_mail)) ? "" : prev_user_mail;

            try
            {
                List<User> userList = await _cloudantService.GetUserAsync(user_mail);
                if (userList != null && userList.Any())
                {
                    foreach (var user in userList)
                    {
                        if (!user.token.Equals(recent_token))
                        {
                            user.token = recent_token;
                            await _cloudantService.UpdateAsync(user);
                        }
                    }
                    return true;
                }
                else
                {
                    User user = new User()
                    {
                        user_mail = user_mail,
                        token = recent_token,
                        push_cekilis = "T",
                        push_win = "T",
                        time = DateTime.Now.ToString()
                    };
                    return await _cloudantService.CreateAsync(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
