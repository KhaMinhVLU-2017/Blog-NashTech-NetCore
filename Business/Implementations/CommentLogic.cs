using System.Collections.Generic;
using System.Linq;
using Business.Services;
using Contracts;
using Entities.DTO;
using Entities.Models;

namespace Business.Implementations
{
    public class CommentLogic : ICommentLogic
    {
        private readonly IRepositoryWrapper _db;
        public CommentLogic(IRepositoryWrapper db){
            _db = db;
        }
        public bool DeleteComment(int commentId)
        {
            try {
                _db.Comments.DeleteById(commentId);
                _db.Save();
                return true;
            }catch{
                return false;
            }
        }

        public ICollection<CommentDTO> SearchComment(string key)
        {
            return _db.Comments.FindByContrain(s => s.Content.Contains(key)).Select(s => new CommentDTO{
                CommentID = s.CommentID,
                Content = s.Content,
                crDate = s.crDate,
                UserID = s.UserID,
                AuthorComment = s.User.Username
            }).ToList();
        }

        public bool UpdateComment(Comment comment)
        {
            try {
                _db.Comments.UpdateEntity(comment);
                _db.Save();
                return true;
            }catch {
                return false;
            }
        }

        public IEnumerable<CommentDTO> ListComment(){
            return _db.Comments.FindByContrain(null).Select(s => new CommentDTO{
           CommentID = s.CommentID,
                Content = s.Content,
                crDate = s.crDate,
                UserID = s.UserID,
                AuthorComment = s.User.Username,
                TitleBlog = s.Blog.Title
            }).ToList();
        }
    }
}