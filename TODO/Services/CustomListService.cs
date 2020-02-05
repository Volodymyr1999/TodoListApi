using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TODO.ViewModels;

namespace TODO.Services
{
    public class CustomListService:Service
    {
        TaskManager _taskManager;
        public CustomListService(Models.TodoContext db,TaskManager taskManager) : base(db) 
        {
            _taskManager = taskManager;
        }
        // method for creating new CustomList by user
        public CustomListView CreateCustomList(Models.User user,string NameCustomList)
        {
            Models.CustomList customList = new Models.CustomList();
            if ((NameCustomList == null) || (NameCustomList == "") || (NameCustomList == " "))
                customList.Name = "CustomList";
            else
                customList.Name = NameCustomList;
            customList.User = user;
            db.customLists.Add(customList);
            db.SaveChanges();

            CustomListView customListView = new CustomListView
            {
                Id = customList.Id,
                Name = customList.Name,
                taskViews = new List<TaskView>()
            };
            return customListView;
        }

        // Convert list<CustomList> to list<CustomListView>
        public List<CustomListView>CreateListCustomViewsFromCustomLists(List<Models.CustomList> customLists,bool IsCompleted)
        {

            var customListViews = customLists.Select(p => new CustomListView
            { Id = p.Id, Name = p.Name, taskViews = _taskManager.Sort(p.Tasks,IsCompleted) });
            return customListViews.ToList();
        }

        // get all customlists created by user and convert them to List<CustomListView>
        //return List<CustomListView>
        public async Task<List<CustomListView>> GetCustomLists(Models.User user,bool IsCompleted)
        {
            var customLists = await db.customLists.Include(p => p.Tasks)
                .ThenInclude(p=>p.Importance).Where(p => p.User == user).ToListAsync();
            var customListViews = CreateListCustomViewsFromCustomLists(customLists,IsCompleted);
            return customListViews;
        }

        //Remove custom list by id (hard remove)
        public async Task RemoveCustomList(int id)
        {
            Models.CustomList customList = await db.customLists.FindAsync(id);
            db.customLists.Remove(customList);
            db.SaveChanges();
        }

        //find custom list by Id and set option return/not retun completed tasks
        //if IsCompleted == true list will include completed tasks
        public async Task<CustomListView>FindCustomList(int id,bool IsCompleted)
        {
            var customList = await db.customLists.Include(p=>p.Tasks).ThenInclude(p=>p.Importance)
                .FirstOrDefaultAsync(p=>p.Id==id);
            CustomListView customListView = new CustomListView
            {
                Id = customList.Id,
                Name = customList.Name,
                taskViews = _taskManager.createTaskViewListFromTaskList(customList.Tasks,IsCompleted)
            };
            return customListView;
        }
        //Rename existing customlist created by user
        public async Task  RenameList(int id,string name)
        {
            Models.CustomList customList =  await db.customLists.Include(p=>p.Tasks).ThenInclude(p=>p.Importance)
                .FirstOrDefaultAsync(p=>p.Id==id);
            customList.Name = name;
            db.SaveChanges();
           
        }

       
    }
}
