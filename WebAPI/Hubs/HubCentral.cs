using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebAPI.Hubs
{
    public class HubCentral: Hub
    {
        private readonly AppMeoContext _db;
        public HubCentral(AppMeoContext db)
        {
            _db = db;
        }
        public async Task SendComment([FromForm]Comment CommentRE)
        {

            var comment = new
            {
                userID = CommentRE.UserID,
                content = CommentRE.Content,
                crDate = CommentRE.crDate,
                commentID = CommentRE.CommentID,
                authorComment = _db.User.Find(CommentRE.UserID).Fullname
            };
            var Comment = JsonConvert.SerializeObject(comment);
            await Clients.All.SendAsync("ReceiveComment", Comment);
        }
    }
}
