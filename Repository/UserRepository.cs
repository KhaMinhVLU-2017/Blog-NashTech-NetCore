using System;
using System.Collections.Generic;
using System.Text;
using Entities.Models;
using Contracts;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>,IUserRepository
    {
        public UserRepository(AppMeoContext options): base(options) { }
    }
}
