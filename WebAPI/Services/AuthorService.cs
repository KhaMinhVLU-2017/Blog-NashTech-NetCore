using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace WebAPI.Services
{
    public interface IAuthorService
    {
        bool CheckAuthorPath(int RoleID,string path);
    }
    public class AuthorService : IAuthorService
    {
        private readonly AppMeoContext _db;
        public AuthorService(AppMeoContext db)
        {
            _db = db;
        }

        public bool CheckAuthorPath(int RoleID,string path)
        {
            var checkPath = _db.ResourcePath.Where(s=>s.RoleID==RoleID && s.Path.Contains(path));
            if(checkPath==null)
            {
                return false;
            }
            return true;
        }
    }
}
