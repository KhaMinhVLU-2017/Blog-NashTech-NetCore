using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Data.SqlClient;
using JWT;
using JWT.Serializers;
using Microsoft.Extensions.Options;
using WebAPI.Helpers;
using Newtonsoft.Json;
using WebAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class BlogController : Controller
    {
        private readonly AppMeoContext _db;
        private readonly AppSettings _Appsettings;

        public BlogController(AppMeoContext db, IOptions<AppSettings> appMeo)
        {
            _db = db;
            _Appsettings = appMeo.Value;
        }

        [HttpPost]
        public IActionResult Create([FromForm]Blog blog, IFormFile file)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    //string Token = Request.Headers["Authorization"];
                    //UserClient decodeUser = JsonConvert.DeserializeObject<UserClient>(DecodeToken(Token, _Appsettings.Secret));

                    UserClient decodeUser = new UserClient();

                    var UserID = HttpContext.Items["UserID"];
                    var RoleID = HttpContext.Items["RoleID"];
                    if (UserID == null)
                    {
                        return Json(new { status = 500, message = "Server Interval" });
                    }
                    decodeUser.UserID = int.Parse(UserID.ToString());
                    decodeUser.RoleID = int.Parse(RoleID.ToString());

                    // Create BLog
                    Blog blogNew = new Blog();
                    blogNew.Content = blog.Content;
                    blogNew.Sapo = blog.Sapo;
                    blogNew.Title = blog.Title;
                    blogNew.crDate = DateTime.Now;
                    blogNew.AuthorID = decodeUser.UserID;
                    if (file != null)
                    {
                        blogNew.Picture = file.FileName;
                        var path = Path.Combine(
                                          Directory.GetCurrentDirectory(), "Assert/Images",
                                          file.FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            stream.Close();
                        }
                    }
                    _db.Blog.Add(blogNew);
                    _db.SaveChanges();
                    transaction.Commit();
                    return Json(new { status = 200, message = "Create Complete" });
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return Json(new { status = 500, message = "Server Interval" + e });
                }
            }
        }

        [HttpGet]
        public IActionResult List()
        {
            var listBlog = _db.Blog.Select(s => new
            {
                s.BlogID,
                s.Title,
                s.Sapo,
                s.Picture,
                s.crDate,
                AuthorName = s.Author.Fullname,
                s.AuthorID
            }).ToList().OrderByDescending(s => s.crDate);
            return Json(new { status = 200, listBlog, message = "Complete" });
        }

        [HttpPost]
        public IActionResult Remove([FromForm]int id)
        {
            try
            {
                var blog = _db.Blog.Find(id);
                var listComment = _db.Comment.Where(s => s.BlogID == id);
                _db.Comment.RemoveRange(listComment);
                _db.Blog.Remove(blog);
                _db.SaveChanges();
                return Json(new { status=200,message="Remove Complete"});
            }
            catch(Exception e)
            {
                return Json(new { status = 500, message = "Server interval" });
            }
        }

        [HttpPost]
        public IActionResult Edit([FromForm]Blog BlogEdit, IFormFile file)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    Blog BlogMeo = _db.Blog.Find(BlogEdit.BlogID);
                    BlogMeo.Sapo = BlogEdit.Sapo;
                    BlogMeo.Title = BlogEdit.Title;
                    BlogMeo.Content = BlogEdit.Content;
                    if (BlogEdit.Picture == "KeepNew")
                    {
                        if (file != null)
                        {
                            BlogMeo.Picture = file.FileName;
                            var path = Path.Combine(
                                              Directory.GetCurrentDirectory(), "Assert/Images",
                                              file.FileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(stream);
                                stream.Close();
                            }
                        }
                        else
                        {
                            BlogMeo.Picture = string.Empty;
                        }
                    }                    
                    _db.Entry(BlogMeo).State = EntityState.Modified;
                    _db.SaveChanges();
                    transaction.Commit();
                    return Json(new { status=200,message="Update complete"});
                    //return Json(BlogMeo);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return Json(new { status = 500, message = "Update failed" });
                }
            }
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            try
            {
                bool Edit = false;
                var UserID = HttpContext.Items["UserID"];
                //Parse ID
                int UserIDToken;
                if (UserID == null)
                {
                    Edit = false;
                    UserIDToken = -1;
                }
                else
                {
                    UserIDToken = int.Parse(UserID.ToString());
                }

                var blogDB = _db.Blog.Find(id);
                if (blogDB == null)
                {
                    return Json(new { status = 200,blog=blogDB, message = "Blog empty" });
                }

                //Compare blog
                if (UserIDToken == blogDB.AuthorID)
                {
                    Edit = true;
                }

                var blog = _db.Blog.Select(s => new
                {
                    s.BlogID,
                    s.Title,
                    s.Sapo,
                    s.Content,
                    s.Picture,
                    s.crDate,
                    edit = Edit,
                    listComment = s.Comment.Select(w => new
                    {
                        w.CommentID,
                        w.Content,
                        w.crDate,
                        w.UserID,
                        AuthorComment = w.User.Fullname
                    }).OrderByDescending(q=>q.crDate).ToList(),
                    AuthorName = s.Author.Fullname,
                    s.AuthorID
                }).FirstOrDefault(s => s.BlogID == id);

                if (blog == null)
                {
                    return Json(new { status = 200, blog, message = "Blog empty" });
                }


                return Json(new { status = 200, blog, message = "Get Blog" });
            }
            catch (Exception e)
            {
                return Json(new { status = 500, blog = "", message = "Server Interval " + e });
            }
        }

        public string DecodeToken(string Token, string KeySecret)
        {
            string token = Token;
            string secret = KeySecret;

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var json = decoder.Decode(token, secret, verify: true);
                Console.WriteLine(json);
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
    }
}