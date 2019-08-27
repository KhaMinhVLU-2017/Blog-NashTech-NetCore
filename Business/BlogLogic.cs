using System;
using Entities.Models;
using Entities.DTO;
using Contracts;
using System.Collections.Generic;

namespace Business
{
    public class BlogLogic
    {
        private IRepositoryWrapper _db;

        public BlogLogic(IRepositoryWrapper db)
        {
            _db = db;
        }

        public void MapTest()
        {
            IEnumerable<User> person = _db.Users.FindByContrain(s=>s.UserID==1);
            //UserView user = person;

        }
    }
}
