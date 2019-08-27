using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities.Models;

namespace Repository
{
    class RoleRepository : RepositoryBase<Role>,IRoleRepository
    {
        public RoleRepository(AppMeoContext options) : base(options) { }
    }
}
