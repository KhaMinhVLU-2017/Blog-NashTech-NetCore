using System;
using Business;
using Business.Services;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Filters;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[AdminAuthor]
    public class AdminController : Controller
    {
        private readonly IBlogLogic _blogService;
        private readonly ICommentLogic _commentService;
        private readonly IRepositoryWrapper _repoWap;
        public AdminController(IBlogLogic blogService, IRepositoryWrapper repoWap, ICommentLogic commentService)
        {
            _blogService = blogService;
            _repoWap = repoWap;
            _commentService = commentService;
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

        #endregion

        #region Comment
        [HttpPost("UpdateComment")]
        public IActionResult EditComment([FromForm]Comment comment)
        {
            bool UpdateCommented = _commentService.UpdateComment(comment);
            if (UpdateCommented)
            {
                return Json(new { status = 200, message = "Update complete" });
            }
            return Json(new { status = 500, message = "Update failed" });
        }
        [HttpPost("DeleteComment")]
        public IActionResult DeleteComment([FromForm]int Id)
        {
            bool deletedComment = _commentService.DeleteComment(Id);
            if (deletedComment)
            {
                return Json(new { status = 200, message = "Delete complete" });
            }
            return Json(new { status = 500, message = "Delete failed" });
        }
        [HttpGet("SearchComment")]
        public IActionResult SearchComment(string keyWord)
        {
            var listComment = _commentService.SearchComment(keyWord);
            return Json(new { status = 200, listComment, message = "Complete" });
        }
        #endregion
    }
}