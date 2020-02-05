using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TODO.Services;

namespace TODO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    
    public class SmartListController : ControllerBase
    {
        Models.TodoContext db; //database
        UserService _userService;
        TaskManager _taskManager;
        readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        //constructor
        public SmartListController(UserService userService,TaskManager taskManager,Models.TodoContext context)
        {
            db = context;
            _userService = userService;
            _taskManager = taskManager;
            _taskManager.setDb(db);
        }

        //get all tasks created by user
        [HttpGet("alltask")]
        public async Task AllTasks(bool completed=false)
        {
            Models.User user = await _userService.FindUser(User);
            var tasks = await _taskManager.getAllTasks(user,completed);
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(tasks, jsonSerializerSettings));
        }
        //get all tasks created by user where DueDate isn't null
        [HttpGet("plannedtasks")]
        public async Task<IActionResult> PlannedTasks(bool completed = false)
        {
            Models.User user = await _userService.FindUser(User);
            var tasks = await _taskManager.getPlannedTasks(user, completed);
            return Ok(JsonConvert.SerializeObject(tasks, jsonSerializerSettings));
        }
        //get all tasks created by user where date of creation is today
        [HttpGet("todaytasks")]
        public async Task<IActionResult>ToDayTasks(bool completed = false)
        {
            Models.User user = await _userService.FindUser(User);
            var tasks = await _taskManager.getToDayTasks(user, completed);
            return Ok(JsonConvert.SerializeObject(tasks, jsonSerializerSettings));
        }

        //get all tasks created by user which have Importance hight or normal
        [HttpGet("importanttasks")]
        public async Task<IActionResult>ImportantTasks(bool completed = false)
        {
            Models.User user = await _userService.FindUser(User);
            var tasks = await _taskManager.getImportantTasks(user, completed);
            return Ok(JsonConvert.SerializeObject(tasks, jsonSerializerSettings));
        }

        
    }
}