using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities.Models;

namespace Repository
{
    public class BlogRepository : RepositoryBase<Blog>, IBlogRepository
    {
    }
}
