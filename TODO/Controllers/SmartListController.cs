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
    /// <summary>
    /// SmartList
    /// </summary>
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
        /// <summary>
        /// conctructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="taskManager"></param>
        /// <param name="context"></param>
        public SmartListController(UserService userService,TaskManager taskManager,Models.TodoContext context)
        {
            db = context;
            _userService = userService;
            _taskManager = taskManager;
            _taskManager.setDb(db);
        }

        /// <summary>
        /// get all tasks created by user
        /// if completed==true return completed tasks
        /// </summary>
        /// <param name="completed"></param>
        /// <returns>list of tasks</returns>
        [HttpGet("alltask")]
        public async Task AllTasks(bool completed=false)
        {
            Models.User user = await _userService.FindUser(User);
            var tasks = await _taskManager.getAllTasks(user,completed);
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(tasks, jsonSerializerSettings));
        }
        /// <summary>
        /// get all tasks created by user where DueDate!=null
        /// if completed==true return completed tasks
        /// </summary>
        /// <param name="completed"></param>
        /// <returns>list of tasks</returns>
        [HttpGet("plannedtasks")]
        public async Task<IActionResult> PlannedTasks(bool completed = false)
        {
            Models.User user = await _userService.FindUser(User);
            var tasks = await _taskManager.getPlannedTasks(user, completed);
            return Ok(JsonConvert.SerializeObject(tasks, jsonSerializerSettings));
        }
        /// <summary>
        /// get all tasks created by user where CreationDate==Today
        /// if completed==true return completed tasks
        /// </summary>
        /// <param name="completed"></param>
        /// <returns>list of tasks</returns>
        [HttpGet("todaytasks")]
        public async Task<IActionResult>ToDayTasks(bool completed = false)
        {
            Models.User user = await _userService.FindUser(User);
            var tasks = await _taskManager.getToDayTasks(user, completed);
            return Ok(JsonConvert.SerializeObject(tasks, jsonSerializerSettings));
        }

        /// <summary>
        /// get all tasks created by user where Importance==hight or normal
        /// if completed==true return completed tasks
        /// </summary>
        /// <param name="completed"></param>
        /// <returns>list of tasks</returns>
        [HttpGet("importanttasks")]
        public async Task<IActionResult>ImportantTasks(bool completed = false)
        {
            Models.User user = await _userService.FindUser(User);
            var tasks = await _taskManager.getImportantTasks(user, completed);
            return Ok(JsonConvert.SerializeObject(tasks, jsonSerializerSettings));
        }

        
    }
}