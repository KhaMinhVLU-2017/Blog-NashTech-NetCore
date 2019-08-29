using System;
using Entities.Models;
using Contracts;
using System.Collections.Generic;
using Entities.DTO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Business
{
    public class BlogLogic : IBlogLogic
    {
        private IRepositoryWrapper _db;

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
            var listBlog = _db.Blogs.SelectCover(s => new BlogDTO
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
            var pathWebAPI = Directory.CreateDirectory("../WebAPI").ToString();
            var path = Path.Combine(pathWebAPI,"Assert/Images",
                              file.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
                stream.Close();
            }
            return file.FileName;
        }

        public bool CreateBlog(Blog blog,object UserID, object RoleID, string namePicture)
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
                    Picture = namePicture
                };

                _db.Blogs.Insert(blogNew);
                _db.Save();
                return true;
            }catch
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

                if(blog.Picture == "KeepNew")
                {
                    BlogMeo.Picture = fileName;
                }
           
                _db.Blogs.Edit(BlogMeo);
                _db.Save();
                return true;
            }catch
            {
                return false;
            }
        }

        public List<BlogDTO> SearchBlogWithKey(string key)
        {
            var listBlog = _db.Blogs.FindByContrain(s => s.Title.Contains(key) || s.Content.Contains(key)).Select(s => new BlogDTO
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
    }
}
