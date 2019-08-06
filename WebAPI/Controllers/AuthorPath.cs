using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthorPath : Controller
    {
        private readonly IAuthorService _author;
        public AuthorPath(IAuthorService Author)
        {
            _author = Author;
        }

        [HttpPost]
        public IActionResult Check(string Path)
        {
            var UserID = HttpContext.Items["UserID"];
            var RoleID = HttpContext.Items["RoleID"];
            if (UserID==null || RoleID==null)
            {
                return Json(new { status = 403, message = "Forhibben" });
            }

            var validpath = _author.CheckAuthorPath(int.Parse(RoleID.ToString()), Path);

            if(validpath)
            {
                return Json(new { status = 200, message = "Allow connect" });
            }
            return Json(new { status = 403, message = "Forhibben" });
        }
    }
}
