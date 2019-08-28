using System;
using System.Collections.Generic;
using System.Text;
using Entities.Models;

namespace Entities.DTO
{
    public class BlogDTO
    {
        public int BlogID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
 
        public string Sapo { get; set; }

        public bool Edit { get; set; }

        public string Picture { get; set; }

        public DateTime crDate { get; set; }

        public virtual int AuthorID { get; set; }

        public virtual List<CommentDTO> ListComment { get; set; }

        public string AuthorName { get; set; }
    }
}
