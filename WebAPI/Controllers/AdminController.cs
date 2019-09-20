using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class AdminController : Controller
    {
        [Route("api/[controller]")]
        #region Post
        public IActionResult GetListPost()
        {
            return null;
        }

        public IActionResult GetPost()
        {
            return null;
        }
        
        public IActionResult EditListPost()
        {
            return null;
        }

        public IActionResult DeletePost()
        {
            return null;
        }
        #endregion

        #region Comment
        public IActionResult GetComment()
        {
            return null;
        }

        public IActionResult GetListComment()
        {
            return null;
        }

        public IActionResult EditComment()
        {
            return null;
        }

        public IActionResult DeleteComment()
        {
            return null;
        }
        #endregion
    }
}