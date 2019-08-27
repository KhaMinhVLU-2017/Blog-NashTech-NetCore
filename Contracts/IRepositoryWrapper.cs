using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IBlogRepository Blogs { get; }
        ICommentRepository Comments { get; }
        IResourcePathRepository ResourcePaths { get; }
        IRoleRepository Roles { get; }
        IUserRepository Users { get; }
        void Save();   
    }
}
