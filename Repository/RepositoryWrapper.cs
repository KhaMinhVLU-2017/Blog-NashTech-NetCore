using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities.Models;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private AppMeoContext _db;
        private IBlogRepository _blog;
        private ICommentRepository _comment;
        private IResourcePathRepository _resource;
        private IRoleRepository _role;
        private IUserRepository _user;

        public RepositoryWrapper(AppMeoContext db)
        {
            _db = db;
        }

        public IBlogRepository Blogs => Blogs == null ? _blog = new BlogRepository(_db) : _blog;

        public ICommentRepository Comments => Comments == null ? _comment = new CommentRepository(_db) : _comment;

        public IResourcePathRepository ResourcePaths => ResourcePaths == null ? _resource = new ResourcePathRepository(_db) : _resource;

        public IRoleRepository Roles => Roles == null ? _role = new RoleRepository(_db) : _role;

        public IUserRepository Users
        {
            get
            {
                if(Users==null)
                {
                    _user = new UserRepository(_db);
                    return _user;
                }
                return _user;
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
