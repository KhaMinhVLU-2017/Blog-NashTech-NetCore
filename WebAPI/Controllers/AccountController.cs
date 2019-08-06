using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;
using System.Text;
using JWT.Algorithms;
using JWT;
using WebAPI.Helpers;
using Microsoft.Extensions.Options;
using JWT.Serializers;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly AppSettings _appsettings;
        private readonly AppMeoContext _db;
        private IUserService _userService; 
        public AccountController(IOptions<AppSettings> appMeo,AppMeoContext db, IUserService service)
        {
            _db = db;
            _userService = service;
            _appsettings = appMeo.Value;
        }

        [HttpPost]
        public IActionResult Register([FromForm]User account)
        {
            if (ModelState.IsValid)
            {
                var checkUsername = _db.User.FirstOrDefault(s => s.Username == account.Username);
                if (checkUsername != null)
                {
                    return Json(new { status = 404, message = "Username is exist" });
                }
                // hashpassword
                string pwhash = BCrypt.Net.BCrypt.HashPassword(account.Password);
                // new User
                User user = new User();
                user.Fullname = account.Fullname;
                user.Password = pwhash;
                user.Username = account.Username;
                user.RoleID = 3;
                // Role member
                _db.User.Add(user);
                _db.SaveChanges();

                // authentication successful so generate jwt token
                var key = Encoding.ASCII.GetBytes(_appsettings.Secret);

                var payload = new Dictionary<string, object>
            {
                 { "UserID", user.UserID },
                 { "RoleID", user.RoleID }
            };

                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();// SHA256 Algorithm
                IJsonSerializer serializer = new JsonNetSerializer();// Convert JSON
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();// Endcode Base 64
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

                var token = encoder.Encode(payload, key);

                return Json(new { status = 200, message = "Create User Complete", fullname = user.Fullname, token = token });
            }
            return Json(new { status=400, message="Field invalid"});
        }

        [HttpPost]
        public IActionResult Login([FromForm]User account)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.Authenticate(account.Username, account.Password);
                if(user == null)
                {
                    return Json(new { status = 400, message = "Username or password invalid" });
                }
                return Json(new { status = 200, message = "Login complete", fullname=user.Fullname,token = user.Token });
            }
            return Json(new { status = 400, message = "Username or password invalid" });
        }

        [HttpGet]
        public IActionResult List()
        {
            return Json("Hello world");
        }

        [HttpGet]
        public IActionResult Mustang()
        {
            return Json("Mustang");
        }
    }
}