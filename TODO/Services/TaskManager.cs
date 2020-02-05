using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TODO;
using TODO.ViewModels;
using System.Reflection;
namespace TODO.Services
{
    /*this is manager for creating, editing,removing and sorting user's tasks*/
    public class TaskManager
    {
        bool order = false;
        public string property = "DueDate";
        Models.TodoContext db;
        
        //allow taskmanager work with database
        public void setDb(Models.TodoContext context)
        {
            db = context;
        }

        //create user's task and appent it to database
        public Models.Task CreateTask(TaskView taskView,Models.User user)
        {
            Models.Task task = new Models.Task
            {
                Description = taskView.Description,
                DueDate = taskView.Date,
                IsCompleted=false,
                Title = taskView.Title,
                CustomListId = taskView.CustomListId,
                CreationDate = DateTime.Now.Date.ToString(),
                UserId=user.Id
            };
            
            string imp = taskView.Importance;
            if (imp == null)
            {
                imp = "normal";
            }
            Models.Importance importance = db.Importances.FirstOrDefault(p => p.Name == imp);
            task.ImportanceId = importance.Id;

            db.Tasks.Add(task);
            db.SaveChanges();
            return task;
        }

        //update task from taskview
        public Models.Task UpdateTask(TaskView taskView)
        {
            Models.Task task = db.Tasks.Find(taskView.Id);
            task.Title = taskView.Title;
            task.Description = taskView.Description;
            task.DueDate = taskView.Date;
            task.CustomListId = task.CustomListId;
            if (taskView.Importance == null)
            {
                task.Importance = db.Importances.FirstOrDefault(p => p.Name == "normal");
            }
            else
                task.Importance = db.Importances.FirstOrDefault(p => p.Name == taskView.Importance);
            db.SaveChanges();
            return task;
        }
        //Remove task by id (Soft delete<->Make completed)
        public async Task RemoveTask(int id)
        {
            Models.Task task = await db.Tasks.FindAsync(id);
            task.IsCompleted = true;
            db.SaveChanges();
        }

        //get all tasks created by user
        private IQueryable<Models.Task>getTasks(Models.User user)
        {

            var tasks = db.Tasks.Include(p => p.Importance).Where(p => p.UserId == user.Id);


            return tasks;
        }
        //Convert list<Task> to list<TaskView>
        public List<TaskView>createTaskViewListFromTaskList(List<Models.Task> tasks,bool IsCompleted)
        {
            
            var taskviews = tasks.Select(p=>new TaskView { 
                Id=p.Id,
                Title=p.Title,
                Description=p.Description,
                Date=p.DueDate,
                CreationDate=p.CreationDate,
                IsComleted=p.IsCompleted,
                Importance=p.Importance.Name,
                CustomListId=p.CustomListId
            });
            
            if(IsCompleted)
                return taskviews.ToList();
            return taskviews.Where(p => p.IsComleted == false).ToList();
            
        }

        
        //get all tasks created by user and sort it
        //if iscompleted==true return completed tasks
        public async Task<List<TaskView>>getAllTasks(Models.User user,bool iscompleted)
        {
            var tasks = await getTasks(user).ToListAsync();
            
            var taskviews = Sort(tasks, iscompleted);
            return taskviews;
        }
        
        //get all planned tasks created by user (DueDate!=null) and sort it
        //if iscompleted==true return completed tasks
        public async Task<List<TaskView>>getPlannedTasks(Models.User user,bool iscompleted)
        {
            var tasks = await getTasks(user).Where(p => p.DueDate != null).ToListAsync();
           

            var taskviews = Sort(tasks, iscompleted);
            return taskviews;
        }
        //get all  tasks created by user where creation date is today and sort it
        //if iscompleted==true return completed tasks
        public async Task<List<TaskView>>getToDayTasks(Models.User user,bool iscompleted)
        {
            var tasks = await getTasks(user).Where(p => p.DueDate == DateTime.Today.ToString("MM/dd/yyyy"))
                .ToListAsync();
            
            var taskviews = Sort(tasks, iscompleted);
            return taskviews;
            
        }
        //Importance==hight|normal
        //get all tasks created by user where Importance is hight or normal and sort it
        //if iscompleted==true return completed tasks
        public async Task<List<TaskView>>getImportantTasks(Models.User user,bool iscompleted)
        {
            var tasks = await getTasks(user).Include(p => p.Importance)
                .Where(p => p.Importance.Name !="low")
                .ToListAsync();
            
            var taskviews = Sort(tasks, iscompleted);
            return taskviews;
            
        }
        //function for sorting
        //Sort by Title|DueDate|State(Completed or active)|Creation date|Importance(low=>normal=>hight)
        //if iscompleted==true return completed tasks
        public List<TaskView>Sort(List<Models.Task>list,bool IsCompleted)
        {


            var Field = typeof(Models.Task).GetProperty(property);
            List<Models.Task> ordlist;
            if(order==false)
                ordlist = list.OrderBy(p => Field.GetValue(p,null)).ToList();
            else
            {
                ordlist = list.OrderByDescending(p => Field.GetValue(p, null)).ToList();
            }
            
            var taskviews = createTaskViewListFromTaskList(ordlist,IsCompleted);

            
            return taskviews;
        }
        //Find Tasks in all lists by name and sort it in different order by different properties
        //if iscompleted==true return completed tasks
        public async Task<List<TaskView>>FindByName(Models.User user,string Name,bool IsCompleted)
        {
            var tasks =await getTasks(user).Where(p => p.Title == Name).ToListAsync();
            var taskviews = Sort(tasks, IsCompleted);

            return taskviews;
        }
        // set sorting ascending(false or true) for all lists created by user in application
        public void setAscending(bool asc)
        {
            order = asc;
        }
        // set default property for sorting lists
        //every list will be sorted by this property
        public void setDefaultProperty(string fieldname)
        {
            property = fieldname;
        }
    }
}
