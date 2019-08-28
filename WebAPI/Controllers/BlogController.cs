using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
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
using System.Drawing;
using LazZiya.ImageResize;
using Contracts;
using System.Transactions;
using Business;
using Entities.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class BlogController : Controller
    {
        private readonly IRepositoryWrapper _db;

        private IBlogLogicService _blogService;

        public BlogController(IRepositoryWrapper db, IBlogLogicService blog)
        {
            _blogService = blog;
            _db = db;
        }

        [HttpPost]
        public IActionResult Create([FromForm]Blog blog, IFormFile file)
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
                Blog blogNew = new Blog
                {
                    Content = blog.Content,
                    Sapo = blog.Sapo,
                    Title = blog.Title,
                    crDate = DateTime.Now,
                    AuthorID = decodeUser.UserID
                };

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
                _db.Blogs.Insert(blogNew);
                _db.Save();
                return Json(new { status = 200, message = "Create Complete" });
            }
            catch (Exception e)
            {
                return Json(new { status = 500, message = "Server Interval" + e });
            }
        }

        [HttpGet]
        public IActionResult List()
        {
            var listBlog = _blogService.GetBlogListDTO();

            return Json(new { status = 200, listBlog, message = "Complete" });
        }

        [HttpPost]
        public IActionResult Remove([FromForm]int id)
        {
            try
            {
                var blog = _db.Blogs.FindByID(id);
                var listComment = _db.Comments.FindByContrain(s => s.BlogID == id);
                _db.Comments.DeleteRange(listComment);
                _db.Blogs.Delete(blog);
                _db.Save();
                return Json(new { status = 200, message = "Remove Complete" });
            }
            catch (Exception e)
            {
                return Json(new { status = 500, message = "Server interval" });
            }
        }

        [HttpPost]
        public IActionResult Edit([FromForm]Blog BlogEdit, IFormFile file)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    Blog BlogMeo = _db.Blogs.FindByID(BlogEdit.BlogID);
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
                    _db.Blogs.Edit(BlogMeo);
                    _db.Save();
                    transaction.Complete();
                    return Json(new { status = 200, message = "Update complete" });
                 
                    //return Json(BlogMeo);
                }
                catch (Exception e)
                {
                    return Json(new { status = 500, message = "Update failed" });
                }
            }
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            try
            {
                bool IsEdit = false;

                var UserID = HttpContext.Items["UserID"];

                bool blogIsNull = _blogService.BlogIsNull(id);

                if (blogIsNull)
                {
                    return Json(new { status = 404, blog = "", message = "Blog empty" });
                }

                IsEdit = _blogService.IsEditBlogWithUserIDBlogID(UserID, id);

                var blog = _blogService.GetDetailBlogWithID(id,IsEdit);

                return Json(new { status = 200, blog, message = "Get Blog" });
            }
            catch (Exception e)
            {
                return Json(new { status = 500, blog = "", message = "Server Interval " + e });
            }
        }

        [HttpGet]
        public IActionResult GetForEdit(int id)
        {
            try
            {
                bool IsEdit = false;

                var UserID = HttpContext.Items["UserID"];

                bool blogIsNull = _blogService.BlogIsNull(id);

                if (blogIsNull)
                {
                    return Json(new { status = 404, blog = "", message = "Blog empty" });
                }

                IsEdit = _blogService.IsEditBlogWithUserIDBlogID(UserID, id);

                var blog = _blogService.GetDetailBlogWithID(id, IsEdit);

                return Json(new { status = 200, blog, message = "Get Blog" });
            }
            catch (Exception e)
            {
                return Json(new { status = 500, blog = "", message = "Server Interval " + e });
            }
        }

        [HttpPost]
        public IActionResult SaveIMG(IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadedImage = Image.FromStream(stream);

                        int height = uploadedImage.Height;
                        int width = uploadedImage.Width;

                        int ratiO = height / width;

                        int widthNew = 500;

                        //returns Image file
                        var img = ImageResize.Scale(uploadedImage, widthNew, widthNew * ratiO);

                        var path = Path.Combine(
                                          Directory.GetCurrentDirectory(), "Assert/ImagesBlog",
                                          file.FileName);

                        img.SaveAs(path);

                        string url = string.Format("http://{0}/assert/imagesblog/{1}", Request.Host.ToString(), file.FileName);

                        return Json(new
                        {
                            status = true,
                            originalName = file.FileName,
                            generatedName = file.FileName,
                            msg = "Image upload successful",
                            imageUrl = url
                        });
                    }
                    //var path = Path.Combine(
                    //                  Directory.GetCurrentDirectory(), "Assert/ImagesBlog",
                    //                  file.FileName);
                    //using (var stream = new FileStream(path, FileMode.Create))
                    //{
                    //    file.CopyTo(stream);
                    //    stream.Close();
                    //}

                    //string url = string.Format("http://{0}/assert/imagesblog/{1}", Request.Host.ToString(), file.FileName);
                    //return Json(new {
                    //    status =true,
                    //    originalName = file.FileName,
                    //    generatedName = file.FileName,
                    //    msg = "Image upload successful",
                    //    imageUrl = url
                    //});
                }
                return Json(new
                {
                    status = false,
                    originalName = "Error",
                    generatedName = "Error",
                    msg = "Image upload failed",
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    status = false,
                    originalName = "Error",
                    generatedName = "Error",
                    msg = "Image upload failed",
                });
            }
        }

        [HttpPost]
        public IActionResult Search([FromForm]string key)
        {
            var listBlog = _db.Blogs.FindByContrain(s => s.Title.Contains(key) || s.Content.Contains(key)).Select(s => new
            {
                s.BlogID,
                s.Title,
                s.Sapo,
                s.Picture,
                s.crDate,
                AuthorName = s.Author.Fullname,
                s.AuthorID
            }).ToList().OrderByDescending(s => s.crDate);
            if (listBlog != null)
            {
                return Json(new { status = 200, listBlog, message = "Complete" });
            }
            return Json(new { status = 404, listBlog = "", message = "Complete" });
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