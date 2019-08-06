using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [MaxLength(20)]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        //FK
        public int RoleID { get; set; }
        public virtual Role Role { get; set; }

        public virtual ICollection<Blog> Blog { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
    }
}
