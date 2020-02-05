using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TODO.Services;
using TODO.ViewModels;
using Xunit;
namespace XUnitTests
{
    public class TaskManagerTest
    {

        TaskManager taskManager = new TaskManager();
        TODO.Models.TodoContext context;
        public TaskManagerTest()
        {
            context= Connector.Connect();
            taskManager.setDb(context);
            ImportanceManager.CreateDefaultImportances(context);
        }
        
        //testing function createtasks
        [Fact]
        public void CreateTask()
        {
            TODO.Models.User user = new TODO.Models.User { UserName = "eejejjjej", Password = "inveein" };
            context.Add(user);
            context.SaveChanges();
            //create one customlist for tasks
            TODO.Models.CustomList customList = new TODO.Models.CustomList { Name = "somename1", User = user };
            context.SaveChanges();
            TaskView taskView = new TaskView 
            { Title = "title", Description = "desct", Date = "06/02/2020", Importance = "low", CustomListId = customList.Id };

            TODO.Models.Task task = taskManager.CreateTask(taskView, user);//testing function

            TODO.Models.Task task1 = context.Tasks.Find(task.Id);
            Assert.NotNull(task1); //if task1 isn't null,it exists in database
        }

        //testing updateTask
        [Fact]
        public void UpdateTaskTest()
        {

            
            TODO.Models.User user = new TODO.Models.User { UserName = "wegenw", Password = "inveein" };
            context.Add(user);
            context.SaveChanges();

            TODO.Models.CustomList customList = new TODO.Models.CustomList { Name = "somename1", User = user };
            context.SaveChanges();

            TaskView taskView = new TaskView { Title = "title", Description = "desct", Date = "06/02/2020", Importance = "low", CustomListId = 1 };
            TODO.Models.Task task= taskManager.CreateTask(taskView, user);//adding to database
            TaskView taskView1 = new TaskView { Id=task.Id,Title = "title1", Description = "desct1", Date = "07/02/2020", Importance = "normal", CustomListId = 1 };
            taskManager.UpdateTask(taskView1);//updating

            TODO.Models.Task task1 = context.Tasks.Include(p=>p.Importance).FirstOrDefault(p=>p.Id==task.Id);

            if ((task1.Title == taskView1.Title) && (task1.Description == taskView1.Description)
                && (task1.DueDate == taskView1.Date) && (task1.Importance.Name == taskView1.Importance))
            {
                Assert.True(true);//if task was updated return true
            }
            else
                Assert.True(false);
        }
       
        //testing RemoveTask
        [Fact]
        public async Task RemoveTaskTest()
        {
            
            TODO.Models.User user = new TODO.Models.User { UserName = "sbfsnffn", Password = "inveein" };
            context.Add(user);
            context.SaveChanges();

            TODO.Models.CustomList customList = new TODO.Models.CustomList { Name = "somename1", User = user };
            context.SaveChanges();

            TaskView taskView = new TaskView { Title = "title", Description = "desct", Date = "06/02/2020", Importance = "low", CustomListId = 1 };
            TODO.Models.Task task = taskManager.CreateTask(taskView, user); //creating

            await taskManager.RemoveTask(task.Id);//deleting
            var task1 = context.Tasks.Find(task.Id);
            Assert.True(task1.IsCompleted);
        }
        
        //testing sort function by properties,ascending
        //if iscompleted==true return completed tasks
        [Theory]
        [InlineData("Title",false,false)]
        [InlineData("DueDate",true,false)]
        [InlineData("Importance",false,true)]
        [InlineData("CreationDate",true,true)]
        [InlineData("IsCompleted",false,true)]
        public void SortTest(string property,bool ascending,bool iscompleted)
        {
            //changing ascending
            taskManager.setAscending(ascending);
            //changing default sorting property
            taskManager.setDefaultProperty(property);
            //creating importances
            var Importances = context.Importances.ToList();

            //create test tasks list
            List<TODO.Models.Task> tasks = new List<TODO.Models.Task> {
                 new TODO.Models.Task{Id=1,Title = "title", Description = "desct", DueDate = "06/02/2020", Importance = Importances[0], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Id=2,Title = "title", Description = "desct", Importance = Importances[2], CustomListId = 1,IsCompleted=true },
                 new TODO.Models.Task{Id=3,Title = "title", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), Importance = Importances[1], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Id=4,Title = "title", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), CustomListId = 1,Importance=Importances[1],IsCompleted=true }
            };
            var ordlist = taskManager.Sort(tasks, iscompleted);//sort funtion

            //sorting tasks by hands
            List<TODO.Models.Task> Orderedtasks;
            if (iscompleted == false)
            {
                Orderedtasks = tasks.Where(p => p.IsCompleted == false).ToList();
            }
            else
                Orderedtasks = tasks;
            if(property=="Title")
            {
                if (!ascending)
                    Orderedtasks = Orderedtasks.OrderBy(p => p.Title).ToList();
                else
                    Orderedtasks = Orderedtasks.OrderByDescending(p => p.Title).ToList();
            }
            else if (property == "DueDate")
            {
                if (!ascending)
                    Orderedtasks = Orderedtasks.OrderBy(p => p.DueDate).ToList();
                else
                    Orderedtasks = Orderedtasks.OrderByDescending(p => p.DueDate).ToList();
            }
            else if (property == "Importance")
            {
                if (!ascending)
                    Orderedtasks = Orderedtasks.OrderBy(p => p.Importance).ToList();
                else
                    Orderedtasks = Orderedtasks.OrderByDescending(p => p.Importance).ToList();
            }
            if (property == "CreationDate")
            {
                if (!ascending)
                    Orderedtasks = Orderedtasks.OrderBy(p => p.CreationDate).ToList();
                else
                    Orderedtasks = Orderedtasks.OrderByDescending(p => p.CreationDate).ToList();
            }
            if (property == "IsCompleted")
            {
                if (!ascending)
                    Orderedtasks = Orderedtasks.OrderBy(p => p.IsCompleted).ToList();
                else
                    Orderedtasks = Orderedtasks.OrderByDescending(p => p.IsCompleted).ToList();
            }
            //convert tasks to taskiews
            var taskviews = Orderedtasks.Select(p => new TaskView
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Date = p.DueDate,
                CreationDate = p.CreationDate,
                IsComleted = p.IsCompleted,
                Importance = p.Importance.Name,
                CustomListId = p.CustomListId
            }).ToList();

            bool x = true;
            for(int i = 0; i < ordlist.Count; i++)
            {
                if (ordlist[i].Id != taskviews[i].Id)
                {
                    x = false;
                    break;
                }
            }

            Assert.True(x);
        }

        //testing  function getAll user's tasks and sorting by properties,ascending
        //if iscompleted==true return completed tasks

        [Theory]
        [InlineData("Title", false, false)]
        [InlineData("DueDate", true, false)]
        [InlineData("Importance", false, true)]
        [InlineData("CreationDate", true, true)]
        [InlineData("IsCompleted", false, true)]
        public async Task getAllTasksTest(string property, bool ascending, bool iscompleted)
        {
            
            taskManager.setAscending(ascending);
            taskManager.setDefaultProperty(property);
            
            TODO.Models.User user = new TODO.Models.User { UserName = "wegenw", Password = "inveein" };
            context.Add(user);
            context.SaveChanges();

            TODO.Models.CustomList customList = new TODO.Models.CustomList { Name = "somename1", User = user };
            context.SaveChanges();

            var Importances = context.Importances.ToList();
            List<TODO.Models.Task> tasks = new List<TODO.Models.Task> {
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = "06/02/2020", Importance = Importances[0], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Title = "title", Description = "desct", Importance = Importances[2], CustomListId = 1,IsCompleted=true },
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), Importance = Importances[1], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), CustomListId = 1,Importance=Importances[1],IsCompleted=true }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();
            var ordtasksviews = await taskManager.getAllTasks(user,iscompleted);

            var myordtasks = context.Tasks.Include(p => p.Importance).Where(p => p.User == user).ToList();
            var myordertaskviews = taskManager.Sort(myordtasks, iscompleted);

            bool x = true;

            for(int i = 0; i < myordertaskviews.Count; i++)
            {
                if (myordertaskviews[i].Id == ordtasksviews[i].Id)
                {
                    x = false;
                    break;
                }
            }

            foreach(var i in context.Tasks)
            {
                context.Tasks.Remove(i);
            }
            context.SaveChanges();
            Assert.True(x);
        }
        [Theory]
        [InlineData("Title", false, false)]
        [InlineData("DueDate", true, false)]
        [InlineData("Importance", false, true)]
        [InlineData("CreationDate", true, true)]
        [InlineData("IsCompleted", false, true)]
        public async Task getPlannedTaskTest(string property, bool ascending, bool iscompleted)
        {
            
            taskManager.setAscending(ascending);
            taskManager.setDefaultProperty(property);

            TODO.Models.User user = new TODO.Models.User { UserName = "wegenw", Password = "inveein" };
            context.Add(user);
            context.SaveChanges();

            TODO.Models.CustomList customList = new TODO.Models.CustomList { Name = "somename1", User = user };
            context.SaveChanges();

            var Importances = context.Importances.ToList();
            List<TODO.Models.Task> tasks = new List<TODO.Models.Task> {
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = "06/02/2020", Importance = Importances[0], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Title = "title", Description = "desct", Importance = Importances[2], CustomListId = 1,IsCompleted=true },
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), Importance = Importances[1], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), CustomListId = 1,Importance=Importances[1],IsCompleted=true }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();

            var ordtasksviews = await taskManager.getPlannedTasks(user, iscompleted);

            var myordtasks = context.Tasks.Include(p => p.Importance).Where(p => p.User == user&&p.DueDate!=null).ToList();
            var myordertaskviews = taskManager.Sort(myordtasks, iscompleted);

            bool x = true;

            for (int i = 0; i < myordertaskviews.Count; i++)
            {
                if (myordertaskviews[i].Id == ordtasksviews[i].Id)
                {
                    x = false;
                    break;
                }
            }

            foreach (var i in context.Tasks)
            {
                context.Tasks.Remove(i);
            }
            context.SaveChanges();
            Assert.True(x);

        }
        //testing getAllToday tasks and sorting by properties,ascending
        //if iscompleted==true return completed tasks
        [Theory]
        [InlineData("Title", false, false)]
        [InlineData("DueDate", true, false)]
        [InlineData("Importance", false, true)]
        [InlineData("CreationDate", true, true)]
        [InlineData("IsCompleted", false, true)]
        public async Task getTodayTaskTest(string property, bool ascending, bool iscompleted)
        {
          
            taskManager.setAscending(ascending);
            taskManager.setDefaultProperty(property);

            TODO.Models.User user = new TODO.Models.User { UserName = "wegenw", Password = "inveein" };
            context.Add(user);
            context.SaveChanges();

            TODO.Models.CustomList customList = new TODO.Models.CustomList { Name = "somename1", User = user };
            context.SaveChanges();

            var Importances = context.Importances.ToList();
            List<TODO.Models.Task> tasks = new List<TODO.Models.Task> {
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = "06/02/2020", Importance = Importances[0], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Title = "title", Description = "desct", Importance = Importances[2], CustomListId = 1,IsCompleted=true },
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), Importance = Importances[1], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), CustomListId = 1,Importance=Importances[1],IsCompleted=true }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();

            var ordtasksviews = await taskManager.getToDayTasks(user, iscompleted);

            var myordtasks = context.Tasks.Include(p => p.Importance)
                .Where(p => p.User == user && p.DueDate ==DateTime.Today.ToString("MM:dd:yyyy")).ToList();
            var myordertaskviews = taskManager.Sort(myordtasks, iscompleted);

            bool x = true;

            for (int i = 0; i < myordertaskviews.Count; i++)
            {
                if (myordertaskviews[i].Id == ordtasksviews[i].Id)
                {
                    x = false;
                    break;
                }
            }

            foreach (var i in context.Tasks)
            {
                context.Tasks.Remove(i);
            }
            context.SaveChanges();
            Assert.True(x);

        }
        //testing getAllImportant tasks and sorting by properties,ascending
        //if iscompleted==true return completed tasks
        [Theory]
        [InlineData("Title", false, false)]
        [InlineData("DueDate", true, false)]
        [InlineData("Importance", false, true)]
        [InlineData("CreationDate", true, true)]
        [InlineData("IsCompleted", false, true)]
        public async Task getImportantTaskTest(string property, bool ascending, bool iscompleted)
        {
            
            taskManager.setAscending(ascending);
            taskManager.setDefaultProperty(property);

            TODO.Models.User user = new TODO.Models.User { UserName = "wegenw", Password = "inveein" };
            context.Add(user);
            context.SaveChanges();

            TODO.Models.CustomList customList = new TODO.Models.CustomList { Name = "somename1", User = user };
            context.SaveChanges();

            var Importances = context.Importances.ToList();
            List<TODO.Models.Task> tasks = new List<TODO.Models.Task> {
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = "06/02/2020", Importance = Importances[0], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Title = "title", Description = "desct", Importance = Importances[2], CustomListId = 1,IsCompleted=true },
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), Importance = Importances[1], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), CustomListId = 1,Importance=Importances[1],IsCompleted=true }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();

            var ordtasksviews = await taskManager.getImportantTasks(user, iscompleted);

            var myordtasks = context.Tasks.Include(p => p.Importance)
                .Where(p => p.User == user&&p.Importance.Name!="low").ToList();
            var myordertaskviews = taskManager.Sort(myordtasks, iscompleted);

            bool x = true;

            for (int i = 0; i < myordertaskviews.Count; i++)
            {
                if (myordertaskviews[i].Id == ordtasksviews[i].Id)
                {
                    x = false;
                    break;
                }
            }

            foreach (var i in context.Tasks)
            {
                context.Tasks.Remove(i);
            }
            context.SaveChanges();
            Assert.True(x);

        }
        //testing getTasksbyTasksName tasks and sorting by properties,ascending
        //if iscompleted==true return completed tasks
        [Theory]
        [InlineData("Title", false, false,"title")]
        [InlineData("DueDate", true, false,"title345")]
        [InlineData("Importance", false, true,"sometask")]
        [InlineData("CreationDate", true, true,"sometask")]
        [InlineData("IsCompleted", false, true,"weweui")]
        public async Task getTasksByNameTest(string property, bool ascending, bool iscompleted,string name)
        {
            
            taskManager.setAscending(ascending);
            taskManager.setDefaultProperty(property);

            TODO.Models.User user = new TODO.Models.User { UserName = "wegenw", Password = "inveein" };
            context.Add(user);
            context.SaveChanges();

            TODO.Models.CustomList customList = new TODO.Models.CustomList { Name = "somename1", User = user };
            context.SaveChanges();

            var Importances = context.Importances.ToList();
            List<TODO.Models.Task> tasks = new List<TODO.Models.Task> {
                 new TODO.Models.Task{Title = "title", Description = "desct", DueDate = "06/02/2020", Importance = Importances[0], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Title = "title", Description = "desct", Importance = Importances[2], CustomListId = 1,IsCompleted=true },
                 new TODO.Models.Task{Title = "title345", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), Importance = Importances[1], CustomListId = 1,IsCompleted=false },
                 new TODO.Models.Task{Title = "sometask", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), CustomListId = 1,Importance=Importances[1],IsCompleted=true },
                 new TODO.Models.Task{Title = "sometask", Description = "desct", DueDate = DateTime.Today.ToString("MM:dd:yyyy"), CustomListId = 1,Importance=Importances[1],IsCompleted=true }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();

            var ordtasksviews = await taskManager.FindByName(user,name, iscompleted);

            var myordtasks = context.Tasks.Include(p => p.Importance)
                .Where(p => p.User == user && p.Title==name).ToList();
            var myordertaskviews = taskManager.Sort(myordtasks, iscompleted);

            bool x = true;

            for (int i = 0; i < myordertaskviews.Count; i++)
            {
                if (myordertaskviews[i].Id == ordtasksviews[i].Id)
                {
                    x = false;
                    break;
                }
            }

            foreach (var i in context.Tasks)
            {
                context.Tasks.Remove(i);
            }
            context.SaveChanges();
            Assert.True(x);

        }
        
       

    }
}
