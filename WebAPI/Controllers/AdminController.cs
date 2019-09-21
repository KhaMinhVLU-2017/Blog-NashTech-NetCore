using System;
using Business;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Filters;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AdminAuthor]
    public class AdminController : Controller
    {
        private readonly IBlogLogic _blogService;
        private readonly IRepositoryWrapper _repoWap;
        public AdminController(IBlogLogic blogService, IRepositoryWrapper repoWap)
        {
            _blogService = blogService;
            _repoWap = repoWap;
        }

        #region Post
        [HttpPost("UpdatePost")]
        public IActionResult EditPost([FromForm]Blog BlogEdit, IFormFile file)
        {
            try
            {
                var namePicture = string.Empty;
                var roleId = HttpContext.Items["RoleID"];
                var roldIdInt = int.Parse(roleId.ToString());
                if (roleId == null || roldIdInt != 1) return Json(new { status = 403, message = "Forbidden" });

                if (file != null)
                {
                    namePicture = _blogService.SaveImageToAssertAndReturnFileName(file);
                }
                bool isUpdateComplete = _blogService.UpdateBlog(BlogEdit, namePicture);
                if (isUpdateComplete)
                {
                    return Json(new { status = 200, message = "Update complete" });
                }
                return Json(new { status = 500, message = "Update failed" });
            }
            catch (Exception e)
            {
                return Json(new { status = 500, message = "Update failed" + e });
            }
        }

        public IActionResult DeletePost([FromForm]int id)
        {
            var roleId = HttpContext.Items["RoleID"];
            var roldIdInt = int.Parse(roleId.ToString());
            if (roleId == null || roldIdInt != 1) return Json(new { status = 403, message = "Forbidden" });
            bool isRemoveBlogTrue = _blogService.RemovePostFromID(id);
            if (isRemoveBlogTrue)
            {
                return Json(new { status = 200, message = "Remove Complete" });
            }
            return Json(new { status = 500, message = "Server interval" });
        }
        [HttpGet("GetLog")]
        public IActionResult SearchBlog()
        {
            _repoWap.Roles.Insert(new Role{Name="SuperMan"});
            _repoWap.Save();
            return null;
        }
        #endregion

        #region Comment

        public IActionResult EditComment()
        {
            return null;
        }

        public IActionResult DeleteComment()
        {
            return null;
        }

        public IActionResult SearchComment()
        {
            return null;
        }
        #endregion
    }
}