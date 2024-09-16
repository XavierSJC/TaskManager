using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using TaskManager.Services;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskManagerController : ControllerBase
    {
        private readonly ILogger<TaskManagerController> _logger;
        private readonly ITaskManagerService _taskManagerService;

        public TaskManagerController(ILogger<TaskManagerController> logger, ITaskManagerService taskManagerService)
        {
            _logger = logger;
            _taskManagerService = taskManagerService;
        }

        [HttpPost("task")]
        public ActionResult Task([FromBody]Models.Task task)
        {
            try
            {
                return Created(string.Empty, _taskManagerService.CreateTask(task));
            }
            catch (Exception ex)
            {
                return BadRequest("Error to create new task: "+ex.Message);
            }
        }

        [HttpGet("tasks")]
        public ActionResult GetTaks()
        {
            return Ok(_taskManagerService.GetAllTask());
        }

        [HttpPut("task")]
        public ActionResult UpdateTask([FromBody] Models.Task task)
        {
            try
            {
                return Ok(_taskManagerService.UpdateTask(task));
            }
            catch (Exception ex)
            {
                return NotFound("Error to create new task: " + ex.Message);
            }
        }

        [HttpDelete("task/{taskId}")]
        public ActionResult DeleteTask(int taskId)
        {
            try
            {
                _taskManagerService.DeleteTask(taskId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound($"taskId {taskId} not found: {ex.Message}");
            }
        }
    }
}
