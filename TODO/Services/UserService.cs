using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using TODO.ViewModels;

namespace TODO.Services
{
    public class UserService:Service
    {
       
        public UserService(Models.TodoContext db) : base(db) { }
       
        //create user from userview
        public Models.User CreateUser(UserView userView)
        {
            Models.User user = new Models.User
            {
                UserName = userView.UserName,
                Password = userView.Password,

            };
            db.Users.Add(user);
            db.SaveChanges();
            return user;

        }
        // find user using username and password
        public async Task<Models.User> FindUser(UserView userView)
        {
            return await db.Users
                .FirstOrDefaultAsync(p => p.UserName == userView.UserName && p.Password == userView.Password);
        }

        //find authorized user in database
        public async Task<Models.User> FindUser(ClaimsPrincipal user)
        {
            
            return await db.Users.Include(p=>p.customLists).ThenInclude(p=>p.Tasks).ThenInclude(p=>p.Importance)
                .FirstOrDefaultAsync(p => p.UserName == user.Identity.Name);
        }
    }
}
