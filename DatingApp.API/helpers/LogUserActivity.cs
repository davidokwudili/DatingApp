using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Datas.IDatas;
using Microsoft.AspNetCore.Mvc.Filters;

using Microsoft.Extensions.DependencyInjection; //so as the GetService should function

namespace DatingApp.API.helpers
{
    //use for save the Last Active Date & Time for a logged in User
    //Basically what this class does is, when ever the UsersController is called, it will login the user last active.
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //get the result from the ActionResult that was just execusted
            var resultContext = await next();

            // get the logged in userId from the Token
            var userId = int.Parse(resultContext.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier).Value);

            //get the IdatingRepo from as a service, case it was declared as a servce in our start.cs file
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            //get the single user
            var user = await repo.GetUser(userId, true);
            //update his/her LastActive
            user.LastActive = DateTime.Now;
            //save changes...
            await repo.SaveAll();
        }

    }
}