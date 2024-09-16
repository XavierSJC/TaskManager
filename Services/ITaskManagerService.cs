namespace TaskManager.Services
{
    public interface ITaskManagerService
    {
        /**
         * The only restriction is the title be Empty or null.
         */
        Models.Task CreateTask(Models.Task task);

        /**
         * 
         */
        Models.Task GetTask(int taskId);

        /**
        * 
        */
        IEnumerable<Models.Task> GetAllTask();

        /**
         * 
         */
        Models.Task UpdateTask(Models.Task task);

        /**
         * 
         */
        void DeleteTask(int taskId);

    }
}
