using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTO
{
    public class CommentDTO
    {
        public int CommentID { get; set; }

        public string Content { get; set; }
        public DateTime crDate { get; set; }

        public int UserID { get; set; }
        
        public string AuthorComment { get; set; }

        public string TitleBlog {get;set;}

        public int BlogId {get;set;}
    }
}
