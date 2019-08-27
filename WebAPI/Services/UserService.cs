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

namespace WebAPI.Services
{
    public interface IUserService
    {
        UserClient Authenticate(string username, string password);
    }
    public class UserService : IUserService
    {
        private readonly AppSettings _appsettings;
        private readonly AppMeoContext _db;

        public UserService(IOptions<AppSettings> appsettings, AppMeoContext db)
        {
            _appsettings = appsettings.Value;
            _db = db;
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

            IDateTimeProvider provider = new UtcDateTimeProvider();
            var now = provider.GetNow();

            var secondsSinceEpoch = UnixEpoch.GetSecondsSince(now.AddMinutes(30));

            var payload = new Dictionary<string, object>
            {
                 { "UserID", checkUsername.UserID },
                 { "RoleID", checkUsername.RoleID },
                    { "exp", secondsSinceEpoch }
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();// SHA256 Algorithm
            IJsonSerializer serializer = new JsonNetSerializer();// Convert JSON
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();// Endcode Base 64
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            
            var token = encoder.Encode(payload, key);
            string[] arrToken = token.Split('.');
            var tokenModifed = string.Format("{0}.{1}{2}.{3}",arrToken[0], _appsettings.Salt, arrToken[1],arrToken[2]);

            UserClient UserNew = new UserClient();
            UserNew.Fullname = checkUsername.Fullname;
            UserNew.Token = tokenModifed;
            return UserNew;
        }
    }
}
