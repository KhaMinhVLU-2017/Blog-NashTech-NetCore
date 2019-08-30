using System;
using System.Collections.Generic;
using System.Text;
using Business;
using Entities.DTO;
using Microsoft.AspNetCore.Http;
using Entities.Models;

namespace UnitTestWebAPI.FakeController
{
    public class BlogControllerFake
    {
        private IBlogLogic _blogservice;

        public BlogControllerFake(IBlogLogic blog)
        {
            _blogservice = blog;
        }

        public BlogDTO GetDetailBlogWithID(int id, bool check)
        {
            return _blogservice.GetDetailBlogWithID(id, check);
        }

        public bool IsEditBlogWithUserIDBlogID(object userID, int blogID)
        {
            return _blogservice.IsEditBlogWithUserIDBlogID(userID, blogID);
        }

        public bool BlogIsNull(int blogID)
        {
            return _blogservice.BlogIsNull(blogID);
        }

        public List<BlogDTO> GetBlogListDTO()
        {
            return _blogservice.GetBlogListDTO();
        }

        public string SaveImageToAssertAndReturnFileName(IFormFile file)
        {
            return _blogservice.SaveImageToAssertAndReturnFileName(file);
        }

        public bool CreateBlog(Blog blog, object UserID, object RoleID, string namePicture)
        {
            return _blogservice.CreateBlog(blog, UserID, RoleID, namePicture);
        }

        public bool UpdateBlog(Blog blog, string fileName)
        {
            return _blogservice.UpdateBlog(blog, fileName);
        }

        public List<BlogDTO> SearchBlogWithKey(string key)
        {
            return _blogservice.SearchBlogWithKey(key);
        }

        public bool RemovePostFromID(int blogID)
        {
            return _blogservice.RemovePostFromID(blogID);
        }

        public string EndcodeTokenWithJWT(User User, byte[] secretKey)
        {
            return _blogservice.EndcodeTokenWithJWT(User, secretKey);
        }
    }
}
