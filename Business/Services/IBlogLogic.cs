using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business
{
    public interface IBlogLogic
    {
        BlogDTO GetDetailBlogWithID(int blogID, bool edit);
        bool IsEditBlogWithUserIDBlogID(object userID, int blogID);
        bool BlogIsNull(int blogID);
        List<BlogDTO> GetBlogListDTO();
    }
}
