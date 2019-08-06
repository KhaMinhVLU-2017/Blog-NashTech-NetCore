using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> User { get; set; }
        public virtual ICollection<ResourcePath> ResourcePath { get; set; }
    }
}
