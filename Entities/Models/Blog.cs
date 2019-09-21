using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string Sapo { get; set; }
        public string Picture { get; set; }
        public bool isComment {get;set;}
        public DateTime crDate { get; set; }
        public bool Visible {get;set;}
        public virtual int AuthorID { get; set; }

        [ForeignKey("AuthorID")]
        public virtual User Author { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }
    }
}
