using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities.Models;

namespace Repository
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(AppMeoContext db) : base(db)  {}
    }
}
