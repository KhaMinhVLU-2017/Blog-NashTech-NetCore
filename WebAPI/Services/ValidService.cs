using JWT;
using JWT.Serializers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using Entities.Models;

namespace WebAPI.Services
{
    public interface IValidService
    {
        string CheckToken(string Token,string KeySecret);
        UserClient UserDecode(UserClient user);
    }

    public class ValidService : IValidService
    {
        private readonly AppSettings _appsettings;

        public ValidService(IOptions<AppSettings> appsettings)
        {
            _appsettings = appsettings.Value;

        }

        public string CheckToken(string Token,string KeySecret)
        {
         

            string[] arrToken = Token.Split(".");
            int lengthSalt = _appsettings.Salt.Length;
            string tokenSecond = arrToken[1].Substring(lengthSalt);

            string token = string.Format("{0}.{1}.{2}",arrToken[0], tokenSecond,arrToken[2]);
            string secret = KeySecret;

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var json = decoder.Decode(token, secret, verify: true);
                return json;
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
                return "Token has expired";
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
                return "Token has invalid signature";
            }
        }

        public UserClient UserDecode(UserClient user)
        {
            UserClient meo = new UserClient();
            meo.UserID = user.UserID;
            meo.RoleID = user.RoleID;
            meo.Token = user.Token;
            meo.Fullname = user.Fullname;
            return meo;
        }
    }
}
