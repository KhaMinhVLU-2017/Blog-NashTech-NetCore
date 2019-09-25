using System.Collections.Generic;
using System.Linq;
using Entities.DTO;
using Entities.Models;

namespace Business.Services
{
    public interface ICommentLogic
    {
        bool UpdateComment(Comment comment);
        
        bool DeleteComment(int commentId);

        ICollection<CommentDTO> SearchComment(string key);

        IEnumerable<CommentDTO> ListComment();
    }
}