using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TODO.Services;
using TODO.ViewModels;

namespace TODO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListController : ControllerBase
    {
        Models.TodoContext db; //database
        TaskManager _taskManager;
        CustomListService _customListService;
        UserService _userService;
        readonly JsonSerializerSettings jsonSerializerSettings =
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        public ListController(Models.TodoContext context,
            TaskManager taskManger,CustomListService customListService,UserService userService)
        {
            db = context;
            _taskManager = taskManger;
            _taskManager.setDb(db);
            ImportanceManager.CreateDefaultImportances(db);//adding defaultImportances
            _customListService = customListService;
            _userService = userService;

            
        }

        [HttpGet("/")] //home page
        public Task get()
        {
            
            return Response.WriteAsync("hello");
        }

        //Create new CustomList and set list's name
        [Authorize]
        [HttpPost("createcustomlist")]
        public async Task CreateCustomList(string NameCustomList)
        {
            Models.User user = await _userService.FindUser(User);
            CustomListView customList = _customListService.CreateCustomList(user, NameCustomList);

            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(customList, jsonSerializerSettings));
        }

        //get all customLists created by user
        [Authorize]
        [HttpGet("getCustomLists")]
        public async Task getCustomLists(bool completed=false)
        {
            Models.User user =await _userService.FindUser(User);
            var lists = await _customListService.GetCustomLists(user,completed); //get lists
           
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(new { lists = lists },jsonSerializerSettings));
        }

        //delete customList by Id after deleting return "deleted"
        [Authorize]
        [HttpDelete("deleteCustomList/{Id}")]
        public async Task deleteCustomList(int Id)
        {
            await _customListService.RemoveCustomList(Id);
            await Response.WriteAsync("deleted");
        }

        //rename customlist
        [Authorize]
        [HttpPost("renamecustomlist")]
        public async Task RenameCustomList(int id,string name)
        {
            await _customListService.RenameList(id, name);
            
            await Response.WriteAsync("List successfully renamed");

        }
        
    }
}