using System;
using System.Collections.Generic;
using System.Text;
using Entities.Models;

namespace Entities.DTO
{
    public class UserView
    {
        public int UserID { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        //FK
        public int RoleID { get; set; }
        public virtual Role Role { get; set; }

        public virtual ICollection<Blog> Blog { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }

        public string Token { get; set; }
        public string Scope { get; set; }
    }
}
