using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities.Models;

namespace Repository
{
    class ResourcePathRepository : RepositoryBase<ResourcePath>, IResourcePathRepository
    {
        public ResourcePathRepository(AppMeoContext option):base(option) { }
    }
}
