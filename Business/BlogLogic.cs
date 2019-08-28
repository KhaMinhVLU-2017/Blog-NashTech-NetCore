using System;
using Entities.Models;
using Contracts;
using System.Collections.Generic;
using Entities.DTO;
using System.Linq;

namespace Business
{
    public class BlogLogic : IBlogLogicService
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

        /**
        * HTTP GET
        * Action List
        */

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
    }
}
