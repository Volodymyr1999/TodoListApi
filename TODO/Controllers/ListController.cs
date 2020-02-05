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
    /// <summary>
    /// ListController
    /// </summary>
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

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="taskManger"></param>
        /// <param name="customListService"></param>
        /// <param name="userService"></param>
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
        /// <summary>
        /// home page
        /// </summary>
        /// <returns>hello</returns>
        [HttpGet("/")] //home page
        public Task get()
        {
            
            return Response.WriteAsync("hello");
        }

        /// <summary>
        /// Create new Custom list
        /// </summary>
        /// <param name="NameCustomList"></param>
        /// <returns>customList</returns>
        [Authorize]
        [HttpPost("createcustomlist")]
        public async Task CreateCustomList(string NameCustomList)
        {
            Models.User user = await _userService.FindUser(User);
            CustomListView customList = _customListService.CreateCustomList(user, NameCustomList);

            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(customList, jsonSerializerSettings));
        }

        /// <summary>
        /// get all customLists created by user
        /// </summary>
        /// <param name="completed"></param>
        /// <returns>object with customlists</returns>
        [Authorize]
        [HttpGet("getCustomLists")]
        public async Task getCustomLists(bool completed=false)
        {
            Models.User user =await _userService.FindUser(User);
            var lists = await _customListService.GetCustomLists(user,completed); //get lists
           
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(new { lists = lists },jsonSerializerSettings));
        }

        /// <summary>
        /// delete customList by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>'deleted'</returns>
        [Authorize]
        [HttpDelete("deleteCustomList/{Id}")]
        public async Task deleteCustomList(int Id)
        {
            await _customListService.RemoveCustomList(Id);
            await Response.WriteAsync("deleted");
        }

        /// <summary>
        /// rename customlist by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns>'List succesfuuly renamed'</returns>
        [Authorize]
        [HttpPost("renamecustomlist")]
        public async Task RenameCustomList(int id,string name)
        {
            await _customListService.RenameList(id, name);
            
            await Response.WriteAsync("List successfully renamed");

        }
        
    }
}