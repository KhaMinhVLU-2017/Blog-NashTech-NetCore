using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentID { get; set; }

        public string Content { get; set; }
        public DateTime crDate { get; set; }

        public int UserID { get; set; }
        public virtual User User { get; set; }
        
        public int BlogID { get; set; }
        public virtual Blog Blog { get; set; }
    }
}
