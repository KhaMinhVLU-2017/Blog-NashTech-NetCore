using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;
using WebAPI.Helpers;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JWT.Algorithms;
using JWT;
using JWT.Serializers;
using Business;

namespace WebAPI.Services
{
    public interface IUserService
    {
        UserClient Authenticate(string username, string password);

        string myEncodeToken(string tokencurren);
    }
    public class UserService : IUserService
    {
        private readonly AppSettings _appsettings;
        private readonly AppMeoContext _db;
        private IBlogLogic _blogservice;

        public UserService(IOptions<AppSettings> appsettings, AppMeoContext db, IBlogLogic blogservice)
        {
            _appsettings = appsettings.Value;
            _db = db;
            _blogservice = blogservice;
        }

        public UserClient Authenticate(string username, string password)
        {
            var checkUsername = _db.User.FirstOrDefault(s => s.Username == username);
            if (checkUsername == null)
            {
                return null;
            }
            // Check password
            bool validPassword = BCrypt.Net.BCrypt.Verify(password, checkUsername.Password);
            if (!validPassword)
            {
                //return Json(new { status = 400, message = "Username or password invalid" });
                return null;
            }

            // authentication successful so generate jwt token
            var key = Encoding.ASCII.GetBytes(_appsettings.Secret);

            var token = _blogservice.EndcodeTokenWithJWT(checkUsername,key);

            if (token == null)
            {
                return null;
            }

            var tokenModifed = myEncodeToken(token);

            UserClient UserNew = new UserClient();
            UserNew.Fullname = checkUsername.Fullname;
            UserNew.Token = tokenModifed;
            return UserNew;
        }

        public string myEncodeToken (string token)
        {
            string[] arrToken = token.Split('.');
            return string.Format("{0}.{1}{2}.{3}", arrToken[0], _appsettings.Salt, arrToken[1], arrToken[2]);
        }
    }
}
