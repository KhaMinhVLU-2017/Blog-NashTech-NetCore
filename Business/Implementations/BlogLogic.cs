﻿using System;
using Entities.Models;
using Contracts;
using System.Collections.Generic;
using Entities.DTO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using JWT.Algorithms;
using JWT;
using JWT.Serializers;

namespace Business
{
    public class BlogLogic : IBlogLogic
    {
        private IRepositoryWrapper _db;
        //private IUserService _userService;
        /// <summary>
        /// Logic of BlogController
        /// </summary>
        /// <param name="db">Service DB</param>
        public BlogLogic(IRepositoryWrapper db)
        {
            _db = db;
        }
        #region Get info blog and Check allow Edit
        /// <summary>
        /// Get infor blog with check role was allow edit or nope
        /// </summary>
        /// <param name="blogID">BlogID</param>
        /// <param name="edit">Boolean Edit</param>
        /// <returns>Object</returns>
        public BlogDTO GetDetailBlogWithID(int blogID, bool edit)
        {
            Blog BlogMain = _db.Blogs.FindByID(blogID);
            var blog = new BlogDTO
            {
                BlogID = BlogMain.BlogID,
                Title = BlogMain.Title,
                Sapo = BlogMain.Sapo,
                Content = BlogMain.Content,
                Picture = BlogMain.Picture,
                crDate = BlogMain.crDate,
                Edit = edit,
                ListComment = BlogMain.Comment.Select(s => new CommentDTO
                {
                    CommentID = s.CommentID,
                    Content = s.Content,
                    crDate = s.crDate,
                    UserID = s.UserID,
                    AuthorComment = s.User.Fullname
                }).OrderByDescending(q => q.crDate).ToList(),
                AuthorName = BlogMain.Author.Fullname,
                AuthorID = BlogMain.AuthorID

            };
            return blog;
        }

        /// <summary>
        /// Check user have role edit blog
        /// </summary>
        /// <param name="userID">UserID</param>
        /// <param name="blogID">BlogID</param>
        /// <returns>Boolean</returns>
        public bool IsEditBlogWithUserIDBlogID(object userID, int blogID)
        {
            try
            {
                if (userID == null)
                {
                    return false;
                }

                int userIDINT = int.Parse(userID.ToString());

                Blog blog = _db.Blogs.FindByID(blogID);

                if (userIDINT != blog.AuthorID)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check Blog null or data
        /// </summary>
        /// <param name="blogID">BlogID</param>
        /// <returns>Boolean</returns>
        public bool BlogIsNull(int blogID)
        {
            var blog = _db.Blogs.FindByID(blogID);
            if (blog == null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Get exist list blog from db
        /// </summary>
        /// <returns>ListObject</returns>
        public List<BlogDTO> GetBlogListDTO()
        {
            var listBlog = _db.Blogs.FindByContrain(s => s.Visible == true).Select(s => new BlogDTO
            {
                BlogID = s.BlogID,
                Title = s.Title,
                Sapo = s.Sapo,
                Picture = s.Picture,
                crDate = s.crDate,
                AuthorName = s.Author.Fullname,
                AuthorID = s.AuthorID
            }).OrderByDescending(s => s.crDate).ToList();
            return listBlog;
        }

        #endregion


        public string SaveImageToAssertAndReturnFileName(IFormFile file)
        {
            try
            {
                var pathWebAPI = Directory.CreateDirectory("../WebAPI").ToString();
                var path = Path.Combine(pathWebAPI, "Assert/Images",
                                  file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Close();
                }
                return file.FileName;
            }catch
            {
                return null;
            }
        }

        public bool CreateBlog(Blog blog, object UserID, object RoleID, string namePicture)
        {
            try
            {
                UserClient decodeUser = new UserClient();

                decodeUser.UserID = int.Parse(UserID.ToString());
                decodeUser.RoleID = int.Parse(RoleID.ToString());

                // Create BLog
                Blog blogNew = new Blog
                {
                    Content = blog.Content,
                    Sapo = blog.Sapo,
                    Title = blog.Title,
                    crDate = DateTime.Now,
                    AuthorID = decodeUser.UserID,
                    Picture = namePicture,
                    Visible= true
                };

                _db.Blogs.Insert(blogNew);
                _db.Save();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool UpdateBlog(Blog blog, string fileName)
        {
            try
            {
                Blog BlogMeo = _db.Blogs.FindByID(blog.BlogID);
                BlogMeo.Sapo = blog.Sapo;
                BlogMeo.Title = blog.Title;
                BlogMeo.Content = blog.Content;

                if (blog.Picture == "KeepNew")
                {
                    BlogMeo.Picture = fileName;
                }

                _db.Blogs.Edit(BlogMeo);
                _db.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<BlogDTO> SearchBlogWithKey(string key)
        {
            var listBlog = _db.Blogs.FindByContrain(s => s.Title.Contains(key) || s.Content.Contains(key) && s.Visible==true).Select(s => new BlogDTO
            {
                BlogID = s.BlogID,
                Title = s.Title,
                Sapo = s.Sapo,
                Picture = s.Picture,
                crDate = s.crDate,
                AuthorName = s.Author.Fullname,
                AuthorID = s.AuthorID
            }).OrderByDescending(s => s.crDate).ToList();
            return listBlog;
        }

        public bool RemovePostFromID(int blogID)
        {
            try
            {
                var blog = _db.Blogs.FindByID(blogID);
                var listComment = _db.Comments.FindByContrain(s => s.BlogID == blogID);
                _db.Comments.DeleteRange(listComment);
                _db.Blogs.Delete(blog);
                _db.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string EndcodeTokenWithJWT(User User, byte[] secretKey)
        {
            try
            {
                IDateTimeProvider provider = new UtcDateTimeProvider();
                var now = provider.GetNow();

                var secondsSinceEpoch = UnixEpoch.GetSecondsSince(now.AddMinutes(30));

                var payload = new Dictionary<string, object>
                {
                 { "UserID", User.UserID },
                 { "RoleID", User.RoleID },
                  { "exp", secondsSinceEpoch }
                };

                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();// SHA256 Algorithm
                IJsonSerializer serializer = new JsonNetSerializer();// Convert JSON
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();// Endcode Base 64
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

                var token = encoder.Encode(payload, secretKey);
                return token;
            }
            catch
            {
                return null;
            }
        }

    }
}
