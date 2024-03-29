﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
using Newtonsoft.Json;
using Business.Services;

namespace WebAPI.Controllers { 
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly AppMeoContext _db;
        private readonly ICommentLogic _commentLogic;
        public CommentController(AppMeoContext db, ICommentLogic commentLogic)
        {
            _db = db;
            _commentLogic = commentLogic;
        }

        [HttpPost]
        public IActionResult Create([FromForm]Comment Comment)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // Add comment
                    var UserID = HttpContext.Items["UserID"];
                    if (User == null)
                    {
                        return Json(new { status = 401, message = "Not found user" });
                    }

                    Comment commentNew = new Comment();
                    commentNew.UserID = int.Parse(UserID.ToString());
                    commentNew.Content = Comment.Content;
                    commentNew.crDate = DateTime.Now;
                    commentNew.BlogID = Comment.BlogID;

                    _db.Comment.Add(commentNew);
                    _db.SaveChanges();

                    var comment = new 
                    {
                        UserID = commentNew.UserID,
                        Content = commentNew.Content,
                        crDate = commentNew.crDate,
                        CommentID = commentNew.CommentID,
                        AuthorComment = _db.User.Find(UserID).Fullname
                    };

                    //var comment = JsonConvert.SerializeObject(commentS);
                    transaction.Commit();
                    return Json(new { status = 200, message = "Complete comment", comment });
                }
                catch
                {
                    transaction.Rollback();
                    return Json(new { status = 500, message = "Server Interval"});
                }
            }
        }

        [HttpPost]
        public IActionResult Remove([FromForm]int commentID)
        {
            try
            {
                var comment = _db.Comment.Find(commentID);
                if (comment == null)
                {
                    return Json(new { status = 404, message = "Not Found" });
                }
                _db.Comment.Remove(comment);
                _db.SaveChanges();
                return Json(new {status=200, message= "Delete complete" });
            }
            catch(Exception e)
            {
                return Json(new { status = 500, message = "Server Interval" + e});
            }
        }

        [HttpPost]
        public IActionResult Edit([FromForm]Comment EditComment)
        {
            try
            {
                var itCom = _db.Comment.Find(EditComment.CommentID);
                if (itCom == null)
                {
                    return Json(new { status = 404, message = "not found" });
                }

                itCom.Content = EditComment.Content;
                _db.SaveChanges();
                return Json(new { status = 200, message = "Edit complete comment" });
            }
            catch(Exception e)
            {
                return Json(new { status = 500, message = "Server Interval" + e});
            }
        }
    }
}