using System;
using Entities.Models;
using Contracts;
using System.Collections.Generic;
using Entities.DTO;
using System.Linq;

namespace Business
{
    public class BlogLogic
    {
        private IRepositoryWrapper _db;

        public BlogLogic(IRepositoryWrapper db)
        {
            _db = db;
        }

        public BlogDTO GetDetailBlogWithID(int id)
        {
            Blog BlogMain = _db.Blogs.FindByID(id);
            var blog = new BlogDTO
            {
                BlogID = BlogMain.BlogID,
                Title = BlogMain.Title,
                Sapo = BlogMain.Sapo,
                Content = BlogMain.Content,
                Picture = BlogMain.Picture,
                crDate = BlogMain.crDate,
                Edit = true,
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

        public bool BlogIsNull(int blogID)
        {
            var blog = _db.Blogs.FindByID(blogID);
            if(blog==null)
            {
                return true;
            }
            return false;
        }
    }
}
