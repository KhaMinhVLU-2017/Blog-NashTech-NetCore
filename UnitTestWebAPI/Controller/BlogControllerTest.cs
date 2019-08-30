using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Contracts;
using Moq;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Business;
using UnitTestWebAPI.FakeController;
using Entities.DTO;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;

namespace UnitTestWebAPI
{
    public class BlogControllerTest
    {
        [Fact]
        public void TestUsingMockDependency()
        {
            // create mock
            var mockDependency = new Mock<IBlogLogic>();

            //Expect
            int blogID = 1;
            bool edit = true;

            // create test
            var sut = new BlogControllerFake(mockDependency.Object).GetDetailBlogWithID(blogID, edit);

            // Assert
            mockDependency.Verify(s => s.GetDetailBlogWithID(blogID, edit), Times.Once);

        }

        #region Input data and expect

        [Theory]
        [InlineData(1, true)]    // BlogID = 1 and Allow Edit
        [InlineData(1, false)]    // BlogID = 1 and Not Allow Edit
        [InlineData(-1, true)]    // BlogID = 1 and Allow Edit
        [InlineData(-1, false)]    // BlogID = 1 and Not Allow Edit
        [InlineData(0, true)]    // BlogID = 1 and Allow Edit
        [InlineData(0, false)]    // BlogID = 1 and Not Allow Edit
        public void Get_DetailBlogWithEdit_ReturnDetailBlogAndEditOrNo(int blogID, bool Edit)
        {
            // Expect
            var blog = new BlogDTO
            {
                BlogID = blogID,
                Title = "DaLat",
                Sapo = "Dalat",
                Content = "Dalat",
                Edit = Edit
            };

            // Arrange
            var mockDependency = new Mock<IBlogLogic>();
            mockDependency.Setup(x => x.GetDetailBlogWithID(It.IsAny<int>(), It.IsAny<bool>()))
                          .Returns(blog);
            // Actual
            var sut = new BlogControllerFake(mockDependency.Object);

            var result = sut.GetDetailBlogWithID(It.IsAny<int>(), It.IsAny<bool>());
            // Assert
            Assert.Equal(blog, result);
        }

        [Theory]
        [InlineData(true)]  // Allow Edit
        [InlineData(false)] // Not Allow edit
        public void User_HaveAllowEditBlog_Boolean(bool expect)
        {
            // Result Fake
            bool isAllowEdit = expect;

            // Arrange
            object userID = 1;
            int blogID = 55;
            var mockdependency = new Mock<IBlogLogic>();
            mockdependency.Setup(s => s.IsEditBlogWithUserIDBlogID(userID, blogID))
                .Returns(isAllowEdit);

            // Actual
            var sut = new BlogControllerFake(mockdependency.Object);

            var result = sut.IsEditBlogWithUserIDBlogID(userID, blogID);

            // Assert
            Assert.Equal(isAllowEdit, result);

        }

        [Fact]
        public void BlogNull_BlogIsNull_True()
        {
            // Expect
            var expect = true;

            // Arrange
            int blogID = int.MaxValue;
            var mockdependency = new Mock<IBlogLogic>();
            mockdependency.Setup(s => s.BlogIsNull(blogID))
                .Returns(expect);

            // Actual
            var sut = new BlogControllerFake(mockdependency.Object);
            var result = sut.BlogIsNull(blogID);

            // Assert
            Assert.Equal(expect, result);

        }

        [Fact]
        public void BlogNotNull_BlogIsNull_False()
        {
            // Expect
            var expect = false;

            // Arrange
            int blogID = int.MaxValue;
            var mockdependency = new Mock<IBlogLogic>();
            mockdependency.Setup(s => s.BlogIsNull(blogID))
                .Returns(expect);

            // Actual
            var sut = new BlogControllerFake(mockdependency.Object);
            var result = sut.BlogIsNull(blogID);

            // Assert
            Assert.Equal(expect, result);
        }

        [Fact]
        public void ListBlogNull_GetListBlogDTO_ListNull()
        {
            // Expect
            List<BlogDTO> blog = new List<BlogDTO>();

            // Arrange
            var mockdependency = new Mock<IBlogLogic>();
            mockdependency.Setup(s => s.GetBlogListDTO())
                .Returns(blog);

            // Actual
            var sut = new BlogControllerFake(mockdependency.Object);
            var result = sut.GetBlogListDTO();

            // Assert
            Assert.Equal(blog, result);
        }
        [Fact]
        public void ListBlog_GetListBlogDTO_List()
        {
            // Expect
            List<BlogDTO> blog = new List<BlogDTO>() {
                new BlogDTO { BlogID=1,Title="Dalat",Sapo="Dalat",Content="Dalat" },
                new BlogDTO { BlogID=2,Title="VungTau",Sapo="Vungtau",Content="VungTau"}
            };

            // Arrange
            var mockdependency = new Mock<IBlogLogic>();
            mockdependency.Setup(s => s.GetBlogListDTO())
                .Returns(blog);

            // Actual
            var sut = new BlogControllerFake(mockdependency.Object);
            var result = sut.GetBlogListDTO();

            // Assert
            Assert.Equal(blog, result);
        }

        public void FileImageNull_SaveIMGBlog_Null()
        {

        }
        [Fact]
        public void FileImage_SaveIMGBlog_NamePicture()
        {
            // Arrange
            // mock inject
            var mockdependency = new Mock<IBlogLogic>();
            //mockfile
            var mockFile = new Mock<IFormFile>();
            string namePicture = "glc.png";
            var myPathIMG = Path.Combine(@"D:\Project\WebAPI\UnitTestWebAPI\Helper\glc.png");
            var sourceIMG = File.OpenRead(myPathIMG);
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(sourceIMG);
            writer.Flush();
            memoryStream.Position = 0;

            mockFile.Setup(f => f.FileName).Returns(namePicture).Verifiable();
            mockFile.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => memoryStream.CopyToAsync(stream))
                .Verifiable();

            // Actual
            var sut = new BlogControllerFake(mockdependency.Object);
            var inputfile = mockFile.Object;

            // Assert
            mockFile.Verify();
        }

        #endregion


        #region Type Input not truth
        [Fact]
        public void CheckInputInt_HaveAllowEditBlog_False()
        {
            // Result Fake
            bool isAllowEdit = false;

            // Arrange
            int userID = 1;
            int blogID = 55;
            var mockdependency = new Mock<IBlogLogic>();
            mockdependency.Setup(s => s.IsEditBlogWithUserIDBlogID(userID, blogID))
                .Returns(isAllowEdit);

            // Actual
            var sut = new BlogControllerFake(mockdependency.Object);

            var result = sut.IsEditBlogWithUserIDBlogID(userID, blogID);

            // Assert
            Assert.Equal(isAllowEdit, result);
        }
        [Fact]
        public void CheckInputString_HaveAllowEditBlog_False()
        {
            // Result Fake
            bool isAllowEdit = false;

            // Arrange
            string userID = "1";
            int blogID = int.MaxValue;
            var mockdependency = new Mock<IBlogLogic>();
            mockdependency.Setup(s => s.IsEditBlogWithUserIDBlogID(userID, blogID))
                .Returns(isAllowEdit);

            // Actual
            var sut = new BlogControllerFake(mockdependency.Object);

            var result = sut.IsEditBlogWithUserIDBlogID(userID, blogID);

            // Assert
            Assert.Equal(isAllowEdit, result);
        }

        [Fact]
        public void CheckInputByte_HaveAllowEditBlog_False()
        {
            // Result Fake
            bool isAllowEdit = false;

            // Arrange
            byte userID = 21;
            int blogID = int.MaxValue;
            var mockdependency = new Mock<IBlogLogic>();
            mockdependency.Setup(s => s.IsEditBlogWithUserIDBlogID(userID, blogID))
                .Returns(isAllowEdit);

            // Actual
            var sut = new BlogControllerFake(mockdependency.Object);

            var result = sut.IsEditBlogWithUserIDBlogID(userID, blogID);

            // Assert
            Assert.Equal(isAllowEdit, result);
        }

        [Fact]
        public void InputByteInt_BlogIsNull_False()
        {
            // Expect
            var expect = false;

            // Arrange
            byte blogID = 95;
            var mockdependency = new Mock<IBlogLogic>();
            mockdependency.Setup(s => s.BlogIsNull(blogID))
                .Returns(false);

            // Actual
            var sut = new BlogControllerFake(mockdependency.Object);
            var result = sut.BlogIsNull(blogID);

            // Assert
            Assert.Equal(expect, result);
        }
        #endregion
    }
}
