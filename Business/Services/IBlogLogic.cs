using Entities.DTO;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business
{
    public interface IBlogLogic
    {
        BlogDTO GetDetailBlogWithID(int blogID, bool edit);
        bool IsEditBlogWithUserIDBlogID(object userID, int blogID);
        bool BlogIsNull(int blogID);
        List<BlogDTO> GetBlogListDTO();

        /// <summary>
        /// Save Picture to Folder Assert in WebAPI project
        /// </summary>
        /// <param name="file">IFormFile</param>
        /// <returns>Picture Name</returns>
        string SaveImageToAssertAndReturnFileName(IFormFile file);

        /// <summary>
        /// Create new blog
        /// </summary>
        /// <param name="blog">Model Blog</param>
        /// <param name="UserID">UserID Object</param>
        /// <param name="RoleID">RoleID Object</param>
        /// <param name="namePicture">Name Picture</param>
        /// <returns>Boolean for Check Save</returns>
        bool CreateBlog(Blog blog, object UserID, object RoleID, string namePicture);

        /// <summary>
        /// Update blog for new content
        /// </summary>
        /// <param name="blog">Model Blog</param>
        /// <param name="fileName">File Name</param>
        /// <returns>Return check blog was update</returns>
        bool UpdateBlog(Blog blog, string fileName);

        /// <summary>
        /// Find post comfortable with keyword
        /// </summary>
        /// <param name="key">KeyWord</param>
        /// <returns>Return list post</returns>
        List<BlogDTO> SearchBlogWithKey(string key);
    }
}
