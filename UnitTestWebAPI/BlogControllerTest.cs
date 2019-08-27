using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using WebAPI.Controllers;
using Contracts;
using Moq;
using WebAPI.Helpers;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace UnitTestWebAPI
{
    public class BlogControllerTest
    {
        [Fact]
        public void Get_DetailBlog_ReturnSameBlogDetail()
        {
            // Arrange
            int fakeBlogID = 5;
            Blog MockBlogDetail = new Blog{
                BlogID = fakeBlogID,
                Title = "dalat",
                Sapo = "dalat",
                crDate = Convert.ToDateTime("2019-08-27 00:00:00.0000000"),
                Content = "dalat",
                Picture = "dalat.jpg"
            };
            var mockRepo = new Mock<IRepositoryWrapper>();

            mockRepo.Setup(s => s.Blogs.FindByID(fakeBlogID)).Returns(MockBlogDetail);
            BlogController controller = new BlogController(mockRepo.Object);
            // Act
            var DetailBlog = controller.Get(fakeBlogID);

            // Asert
            Assert.NotNull(DetailBlog);
        }
    }
}
