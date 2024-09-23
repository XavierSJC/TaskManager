using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Services;

namespace ServicesTests
{
    public class CRUDTasksTests
    {
        private TaskContext _dbContext;
        private List<TaskManager.Models.Task> defaultTasks;

        [SetUp]
        public void Setup()
        {
            SetupDBContext();
            LoadDefaultData();
        }

        [TearDown]
        public void Teardown()
        {
            _dbContext.Dispose();
            defaultTasks.Clear();
        }

        [Test]
        public void Create_New_Complete_Task()
        {
            var service = new TaskManagerService(_dbContext);
            var newTask = new TaskManager.Models.Task()
            {
                Title = "New Task tile",
                Description = "Description of the new task",
                IsCompleted = true,
                DueDate = DateTime.Now,
            };

            var taskCreated = service.CreateTask(newTask);

            Assert.That(taskCreated.Title, Is.EqualTo(newTask.Title), "Title of the task created");
            Assert.That(taskCreated.Description, Is.EqualTo(newTask.Description), "Description of the task created");
            Assert.That(taskCreated.IsCompleted, Is.EqualTo(newTask.IsCompleted), "Completed flag of the task created");
            Assert.That(taskCreated.DueDate, Is.EqualTo(newTask.DueDate), "Due date of the task created");
            Assert.That(taskCreated.Id, Is.EqualTo(4), "Id of the task created");
        }

        [Test]
        public void Create_Task_Blank_Title_Throw_Exeception()
        {
            var service = new TaskManagerService(_dbContext);
            var newTask = new TaskManager.Models.Task()
            {
                Title = ""
            };

            Assert.Catch<ArgumentNullException>(() => service.CreateTask(newTask));
        }

        [Test]
        public void Create_Task_Only_Title()
        {
            var service = new TaskManagerService(_dbContext);
            var newTask = new TaskManager.Models.Task()
            {
                Title = "Only title"
            };

            var taskCreated = service.CreateTask(newTask);

            Assert.That(taskCreated.Title, Is.EqualTo(newTask.Title), "Title of the task created");
            Assert.That(taskCreated.Description, Is.EqualTo(newTask.Description), "Description of the task created");
            Assert.That(taskCreated.IsCompleted, Is.EqualTo(newTask.IsCompleted), "Completed flag of the task created");
            Assert.That(taskCreated.DueDate, Is.EqualTo(newTask.DueDate), "Due date of the task created");
            Assert.That(taskCreated.Id, Is.EqualTo(4), "Id of the task created");
        }

        [Test]
        public void Update_Only_Title_Task()
        {
            const int idTask = 1;
            const string newTitle = "New Title";
            var service = new TaskManagerService(_dbContext);

            var updatedTask = defaultTasks[idTask];
            updatedTask.Title = newTitle;

            updatedTask = service.UpdateTask(updatedTask);

            Assert.That(updatedTask.Title, Is.EqualTo(newTitle), "Title of the task updated");
            Assert.That(updatedTask.Description, Is.EqualTo(defaultTasks[idTask].Description), "Description of the task not updated");
            Assert.That(updatedTask.IsCompleted, Is.EqualTo(defaultTasks[idTask].IsCompleted), "Completed flag of the task not updated");
            Assert.That(updatedTask.DueDate, Is.EqualTo(defaultTasks[idTask].DueDate), "Due date of the task not updated");
            Assert.That(updatedTask.Id, Is.EqualTo(defaultTasks[idTask].Id), "Id of the task not updated");
        }

        [Test]
        public void Update_All_Task()
        {
            var service = new TaskManagerService(_dbContext);
            var updatedTask = new TaskManager.Models.Task()
            {
                Id = 2,
                Title = "Task tile updated",
                Description = "Description of the task changed",
                IsCompleted = true,
                DueDate = DateTime.Now,
            };

            var savedTask = service.UpdateTask(updatedTask);

            Assert.That(updatedTask.Title, Is.EqualTo(savedTask.Title), "Title of the task updated");
            Assert.That(updatedTask.Description, Is.EqualTo(savedTask.Description), "Description of the task updated");
            Assert.That(updatedTask.IsCompleted, Is.EqualTo(savedTask.IsCompleted), "Completed flag of the task updated");
            Assert.That(updatedTask.DueDate, Is.EqualTo(savedTask.DueDate), "Due date of the task updated");
            Assert.That(updatedTask.Id, Is.EqualTo(savedTask.Id), "Id of the task updated");
        }

        [Test]
        public void Update_Invalid_Task_Throw_Exception()
        {
            var service = new TaskManagerService(_dbContext);
            var updatedTask = new TaskManager.Models.Task()
            {
                Id = 99,
                Title = "Task tile updated"
            };

            Assert.Catch<Exception>(() => service.UpdateTask(updatedTask));
        }

        [Test]
        public void Delete_Valid_Task_By_Id()
        {
            var service = new TaskManagerService(_dbContext);

            service.DeleteTask(2);

            var allTaks = _dbContext.Tasks.ToList();
            Assert.That(allTaks.Count, Is.EqualTo(2), "One taks was removed in the DB");
            
        }

        [Test]
        public void Delete_Invalid_Task_By_Id()
        {
            var service = new TaskManagerService(_dbContext);

            service.DeleteTask(99);

            var allTaks = _dbContext.Tasks.ToList();
            Assert.That(allTaks.Count, Is.EqualTo(3), "No taks was removed in the DB");

        }

        [Test]
        public void Read_Task_By_Id()
        {
            const int idTask = 1;
            var service = new TaskManagerService(_dbContext);

            var task = service.GetTask(1);

            Assert.That(task.Id, Is.EqualTo(defaultTasks[idTask - 1].Id), "Id task match with the default DB");
            Assert.That(task.Title, Is.EqualTo(defaultTasks[idTask - 1].Title), "Id task match with the default DB");
            Assert.That(task.Description, Is.EqualTo(defaultTasks[idTask - 1].Description), "Id task match with the default DB");
            Assert.That(task.IsCompleted, Is.EqualTo(defaultTasks[idTask - 1].IsCompleted), "Id task match with the default DB");
            Assert.That(task.DueDate, Is.EqualTo(defaultTasks[idTask - 1].DueDate), "Id task match with the default DB");
        }

        [Test]
        public void Read_Invalid_Task_Should_Return_Null()
        {
            var service = new TaskManagerService(_dbContext);

            var task = service.GetTask(99);

            Assert.That(task, Is.Null, "Returned null to invalid task");
        }

        [Test]
        public void Read_All_Task()
        {
            var service = new TaskManagerService(_dbContext);

            var task = service.GetAllTask();

            Assert.That(task.Count(), Is.EqualTo(3), "Returned all the tasks");
        }

        private void SetupDBContext() 
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            var optionsBuilder = new DbContextOptionsBuilder<TaskContext>().UseSqlite(conn);
            _dbContext = new TaskContext(optionsBuilder.Options);
            _dbContext.Database.EnsureCreated();

        }

        private void LoadDefaultData()
        {
            defaultTasks = new List<TaskManager.Models.Task>();

            defaultTasks.Add(new TaskManager.Models.Task()
            {
                Title = "Task tile 1",
                Description = "Description of the new task 1",
                IsCompleted = true,
                DueDate = DateTime.Now,
            });

            defaultTasks.Add(new TaskManager.Models.Task()
            {
                Title = "Task tile 2",
                Description = "Description of the new task 2",
                IsCompleted = false,
                DueDate = DateTime.Now,
            });

            defaultTasks.Add(new TaskManager.Models.Task()
            {
                Title = "Task tile 3"
            });

            _dbContext.Tasks.AddRange(defaultTasks);
            _dbContext.SaveChanges();
        }
    }
}