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

        // create task from taskView
        [HttpPost("/createtask")]

        public async Task< IActionResult> CreateTask(TaskView taskView)
        {
            Models.User user = await _userService.FindUser(User);
            Models.Task task = _taskManager.CreateTask(taskView,user);
            
            return Ok(new { info = "created" });
        }
        
        //update existing task using taskView
        [HttpPut("/updatetask")]
        public IActionResult UpdateTask(TaskView taskView)
        {
            Models.Task task = _taskManager.UpdateTask(taskView);
            return Ok("updated");
        }

        //delete multiple task by id
        [HttpDelete("/deletetask")]
        public async Task<IActionResult>DeleteTask(List<int>taskIds)
        {
            foreach(var id in taskIds)
                await _taskManager.RemoveTask(id);

            return Ok("deleted");
        }
        //find task by name in all lists and return it in taskview object
        [HttpGet("/findtaskbyname")]
        public async Task GetTasksByName(string Name,bool completed)
        {
            Models.User user =await _userService.FindUser(User);
            var tasks = _taskManager.FindByName(user, Name, completed);
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(tasks, jsonSerializerSettings));
        }

        //set sort order all user's list in application
        //all next task's queries will be sorted by this order
        [HttpGet("/setorder")]
        public async Task setOrder(string fieldname,bool ascending=false)
        {
            
            _taskManager.setAscending(ascending);
            _taskManager.setDefaultProperty(fieldname);
            await Response.WriteAsync(_taskManager.property);
        }
    }
}