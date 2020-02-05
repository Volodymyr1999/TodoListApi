using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TODO.Services;
using TODO.ViewModels;

namespace TODO.Controllers
{
    /// <summary>
    /// TaskController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        Models.TodoContext db;//database
        TaskManager _taskManager;
        UserService _userService;
        readonly JsonSerializerSettings jsonSerializerSettings =
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        public TaskController(TaskManager taskManager,UserService userService,Models.TodoContext context)
        {
            db = context;
            _taskManager = taskManager;
            _taskManager.setDb(db);
            _userService = userService;
        }

        /// <summary>
        /// create task from model taskview
        /// </summary>
        /// <param name="taskView"></param>
        /// <returns>'created'</returns>
        [HttpPost("createtask")]

        public async Task< IActionResult> CreateTask(TaskView taskView)
        {
            Models.User user = await _userService.FindUser(User);
            Models.Task task = _taskManager.CreateTask(taskView,user);
            
            return Ok(new { info = "created" });
        }
        
        /// <summary>
        /// uodate task by taskView
        /// </summary>
        /// <param name="taskView"></param>
        /// <returns>'updated'</returns>
        [HttpPut("updatetask")]
        public IActionResult UpdateTask(TaskView taskView)
        {
            Models.Task task = _taskManager.UpdateTask(taskView);
            return Ok("updated");
        }

        /// <summary>
        /// delete multiple tasks by id
        /// </summary>
        /// <param name="taskIds"></param>
        /// <returns>'deletedl</returns>
        [HttpDelete("deletetask")]
        public async Task<IActionResult>DeleteTask(List<int>taskIds)
        {
            foreach(var id in taskIds)
                await _taskManager.RemoveTask(id);

            return Ok("deleted");
        }
        /// <summary>
        /// find task in all lists created bu user by name
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="completed"></param>
        /// <returns>list of TaskView</returns>
        [HttpGet("findtaskbyname")]
        public async Task GetTasksByName(string Name,bool completed=false)
        {
            Models.User user =await _userService.FindUser(User);
            var tasks = _taskManager.FindByName(user, Name, completed);
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(tasks, jsonSerializerSettings));
        }

       /// <summary>
       /// set sort order for all lists creted by user in api
       /// next operations with this lists will have this sorting order
       /// </summary>
       /// <param name="fieldname"></param>
       /// <param name="ascending"></param>
       /// <returns></returns>
        [HttpGet("setorder")]
        public async Task setOrder(string fieldname,bool ascending=false)
        {
            
            _taskManager.setAscending(ascending);
            _taskManager.setDefaultProperty(fieldname);
            await Response.WriteAsync(_taskManager.property);
        }
    }
}