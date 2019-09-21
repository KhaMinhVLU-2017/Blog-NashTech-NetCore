using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace WebAPI.Filters
{
    public class AdminAuthor : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Items["UserID"];
            var roleId = context.HttpContext.Items["RoleID"];
            if (roleId == null)
            {
                string jsonString = JsonConvert.SerializeObject(new
                {
                    status = 401,
                    message = "UnAuthorization"
                });
                context.Result = new JsonResult(jsonString);
            }
            else
            {
                var intRoleId = int.Parse(roleId.ToString());
                if (intRoleId != 1)
                {
                    string jsonString = JsonConvert.SerializeObject(new
                    {
                        status = 403,
                        message = "Forbidden"
                    });
                    context.Result = new JsonResult(jsonString);
                }
            }
        }
    }
}