using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using TODO;
using TODO.Services;
using TODO.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace XUnitTests
{
    public class UserServiceTest
    {
        UserService _usetService;
        
        
       
        [Fact]

        //testing adding user to database
        public void CreateUserFromUserViewTest()
        {
            var context = Connector.Connect();
            _usetService = new UserService(context);
            UserView userView = new UserView
            {
                UserName = "Volodymyr",
                Password = "ertywerty"
            };

            TODO.Models.User user =_usetService.CreateUser(userView);
            TODO.Models.User user1 = context.Users.FirstOrDefault(p => p.UserName == user.UserName && p.Password == user.Password);

            Assert.True(user1.Id == user.Id);
            
        }
        //testing FindUserbyUserView
        [Fact] 
        public async Task FindUserByUserViewTest()
        {
            var context = Connector.Connect();
            _usetService = new UserService(context);
           
            UserView usrwiew = new UserView { UserName = "Vasya", Password = "wwweibnwi" };
            TODO.Models.User user = _usetService.CreateUser(usrwiew);
            TODO.Models.User user1 = await _usetService.FindUser(usrwiew);

            Assert.True(user.Id == user1.Id);

        }
      
    }
}
