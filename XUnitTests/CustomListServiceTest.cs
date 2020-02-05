using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TODO.Services;
using System.Threading.Tasks;
using System.Linq;
using TODO.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace XUnitTests
{
    public class CustomListServiceTest
    {
        TaskManager taskManager;
        CustomListService listService;
        TODO.Models.TodoContext context;
        public CustomListServiceTest()
        {
            taskManager = new TaskManager();
            context = Connector.Connect();
            taskManager.setDb(context); 
            listService = new CustomListService(context,taskManager);
            ImportanceManager.CreateDefaultImportances(context);
        }
        
        //testing function RenameCustomList

        [Fact]
        public async Task RenameCustomListTest()
        {
            taskManager = new TaskManager();
            context = Connector.Connect();
            taskManager.setDb(context);
            listService = new CustomListService(context, taskManager);
            TODO.Models.User user = new TODO.Models.User { UserName = "Vasya", Password = "ainivn" };
            context.Users.Add(user);
            context.SaveChanges();

            var customList = listService.CreateCustomList(user, "customlist");
            
            await listService.RenameList(customList.Id, "customlist123");//testing function

            var customList123 = context.customLists.Find(customList.Id);

            Assert.Equal("customlist123",customList123.Name);
        }

        //testing function RemoveCustomList
        [Fact]
        public async Task RemoveCustomListTest()
        {
            taskManager = new TaskManager();
            context = Connector.Connect();
            taskManager.setDb(context);
            listService = new CustomListService(context, taskManager);
            TODO.Models.User user = new TODO.Models.User { UserName = "fqfquguq", Password = "ainivn" };
            context.Users.Add(user);
            context.SaveChanges();

            var customList = listService.CreateCustomList(user, "customlist25622");

            await listService.RemoveCustomList(customList.Id); //testing function

            var custlist = context.customLists.Find(customList.Id);

            Assert.Null(custlist);

        }
        
        //testing create customlist
        [Fact]
        public void CreateCustomListTest()
        {
            
            TODO.Models.User user = new TODO.Models.User { UserName = "esbss", Password = "ainivn" };
            context.Users.Add(user);
            context.SaveChanges();
            var customListView = listService.CreateCustomList(user, "SomeList");//testing function
            var cust1 = context.customLists.Find(customListView.Id);
            
            Assert.NotNull(cust1);
        }

        //testing FindCustomListById
        //if Iscompleted==true return completed tasks
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task FindCustonListById(bool IsCompleted)
        {
            TODO.Models.User user = new TODO.Models.User { UserName = "esbss", Password = "ainivn" };
            context.Users.Add(user);
            context.SaveChanges();
            
            var customListView = listService.CreateCustomList(user, "SomeList");
            var cust1 = await listService.FindCustomList(customListView.Id, IsCompleted); //testint function
            var cust2 = context.customLists.Find(customListView.Id);
            Assert.Equal(cust2.Id, cust1.Id);
        }

        //testing function  CreateListCustomViewsFromCustomListsTest
        ////if Iscompleted==true return completed tasks
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CreateListCustomViewsFromCustomListsTest(bool iscompleted)
        {
            
            TODO.Models.User user = new TODO.Models.User { UserName = "fqfquguq", Password = "ainivn" };
            context.Users.Add(user);
            context.SaveChanges();

            //creation testing task's list
            List<TODO.Models.Task> tasks = new List<TODO.Models.Task>
            {
                new TODO.Models.Task
                    {
                        Id=1,
                        Title="Title",
                        Description="Desc",
                        DueDate="06/02/2020",
                        IsCompleted=false,
                        CreationDate=DateTime.Today.ToString("MM:dd:yyyy"),
                        Importance=context.Importances.Find(2),
                        CustomListId=1,
                        UserId=1

                    },
                    new TODO.Models.Task
                    {
                        Id=2,
                        Title="Title1",
                        Description="Desc1",

                        IsCompleted=false,
                        CreationDate=DateTime.Today.ToString("MM:dd:yyyy"),
                        Importance=context.Importances.Find(1),
                        CustomListId=1,
                        UserId=1

                    },
                    new TODO.Models.Task
                    {
                        Id=3,
                        Title="Title2",
                        Description="Desc2",
                        DueDate="06/02/2020",
                        IsCompleted=true,
                        CreationDate=DateTime.Today.ToString("MM:dd:yyyy"),
                        Importance=context.Importances.Find(1),
                        CustomListId=1,
                        UserId=1

                    },
            };
            //creation testing list of customlists
            List<TODO.Models.CustomList> customLists = new List<TODO.Models.CustomList>
            {
                new TODO.Models.CustomList{Id=1,Name="wvwvu",UserId=1,Tasks=tasks},
                new TODO.Models.CustomList{Id=2,UserId=1,Name="aniwnw",Tasks=new List<TODO.Models.Task>()}
            };
            ;
            var lists = customLists.Select(p => new CustomListView
            {
                Id = p.Id,
                Name = p.Name,
                taskViews = taskManager.Sort(p.Tasks, iscompleted)
            }).ToList();
            var customListViews = listService.CreateListCustomViewsFromCustomLists(customLists, iscompleted);

            bool x = true;
            for (int i = 0; i < customListViews.Count; i++)
            {
                if ((customListViews[i].Id != lists[i].Id) || (customListViews[i].Name != lists[i].Name))
                {
                    x = false;
                    break;
                }
                for(int j = 0; j < customListViews[i].taskViews.Count; j++)
                {
                    if (customListViews[i].taskViews[j].Id != lists[i].taskViews[j].Id)
                    {
                        x = false;
                        break;
                    }
                    
                }
                if (x==false)
                    break;

            }
            Assert.True(x);
        }
        //testing GetCustomLists
        //if Iscompleted==true return completed tasks
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetCustomListTest(bool IsCompleted)
        {

            TODO.Models.User user = new TODO.Models.User { UserName = "egeg", Password = "ainivn" };
            context.Users.Add(user);
            context.SaveChanges();

            //create test task's list
            List<TODO.Models.Task> tasks = new List<TODO.Models.Task>
            {
                new TODO.Models.Task
                    {

                        Title="Title",
                        Description="Desc",
                        DueDate="06/02/2020",
                        IsCompleted=false,
                        CreationDate=DateTime.Today.ToString("MM:dd:yyyy"),
                        Importance=context.Importances.Find(2),
                        CustomListId=1,
                        UserId=user.Id

                    },
                    new TODO.Models.Task
                    {

                        Title="Title1",
                        Description="Desc1",

                        IsCompleted=false,
                        CreationDate=DateTime.Today.ToString("MM:dd:yyyy"),
                        Importance=context.Importances.Find(1),
                        CustomListId=1,
                        UserId=user.Id

                    },
                    new TODO.Models.Task
                    {

                        Title="Title2",
                        Description="Desc2",
                        DueDate="06/02/2020",
                        IsCompleted=true,
                        CreationDate=DateTime.Today.ToString("MM:dd:yyyy"),
                        Importance=context.Importances.Find(1),
                        CustomListId=1,
                        UserId=user.Id

                    },
            };

            //add tasks to database
            context.Tasks.AddRange(tasks);
            context.SaveChanges();
            //create test list of customllists
            List<TODO.Models.CustomList> customLists = new List<TODO.Models.CustomList>
            {
                new TODO.Models.CustomList{Name="wvwvu",UserId=user.Id,Tasks=tasks},
                new TODO.Models.CustomList{UserId=user.Id,Name="aniwnw",Tasks=new List<TODO.Models.Task>()}
            };
            //add list of customlists to database
            context.customLists.AddRange(customLists);
            context.SaveChanges();

            var custlistviews = await listService.GetCustomLists(user, IsCompleted);//testing function

            var lists = await context.customLists.Include(p => p.Tasks)
                 .ThenInclude(p => p.Importance).Where(p => p.User == user).ToListAsync();
            var myCustomListViews = listService.CreateListCustomViewsFromCustomLists(lists, IsCompleted);

            //testing mycustomListsView and customListsView
            bool x = true;
            for(int i = 0; i < custlistviews.Count; i++)
            {
                if ((custlistviews[i].Id != myCustomListViews[i].Id)||(custlistviews[i].Name!=myCustomListViews[i].Name))
                {
                    x = false;
                    break;
                }
                for(int j = 0; j < custlistviews[i].taskViews.Count; j++)
                {
                    if (custlistviews[i].taskViews[j].Id != myCustomListViews[i].taskViews[j].Id)
                    {
                        x = false;
                        break;
                    }
                }
                if (x == false)
                    break;
            }

            Assert.True(x);
        }
    }
}
