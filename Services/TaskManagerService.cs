
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;

namespace TaskManager.Services
{
    public class TaskManagerService : ITaskManagerService
    {
        private readonly TaskContext _dbContext;

        public TaskManagerService(TaskContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Models.Task CreateTask(Models.Task task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            if (string.IsNullOrEmpty(task.Title)) throw new ArgumentNullException("Description of the Task");

            _dbContext.Tasks.Add(task);
            _dbContext.SaveChanges();
            return task;
        }

        public void DeleteTask(int taskId)
        {
            _dbContext.Tasks.Where(t => t.Id == taskId).ExecuteDelete();
        }

        public IEnumerable<Models.Task> GetAllTask()
        {
            return _dbContext.Tasks.ToList();
        }

        public Models.Task GetTask(int taskId)
        {
            return _dbContext.Tasks.Where(t => t.Id == taskId).FirstOrDefault();
        }

        public Models.Task UpdateTask(Models.Task task)
        {
            var taskUpdate = _dbContext.Tasks.Where(t => t.Id == task.Id).FirstOrDefault();
            if (taskUpdate == null) throw new Exception("Task not found");

            if (!string.IsNullOrEmpty(task.Title)) taskUpdate.Title = task.Title;
            if (!string.IsNullOrEmpty(task.Description)) taskUpdate.Description = task.Description;
            if (task.DueDate != null) taskUpdate.DueDate = task.DueDate;
            if (task.IsCompleted != taskUpdate.IsCompleted) taskUpdate.IsCompleted = task.IsCompleted;

            _dbContext.SaveChanges();

            return taskUpdate;
        }
    }
}
