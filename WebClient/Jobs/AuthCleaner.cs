using Quartz;

namespace WebClient.Jobs;

public class AuthCleaner : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var task = Task.Run(() =>
        {
            Console.WriteLine("Do work");
            //todo:
        });
        return task;
    }
}