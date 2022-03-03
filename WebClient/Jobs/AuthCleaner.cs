using System.Diagnostics;
using DB.Repositories.Computer;
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
            //var c = new ComputerRepositoryDapper();
            //var qq = new DB.Entities.Computer()
            //{
            //    Date = DateTime.UtcNow
            //};
            //c.CreateComputer(qq);
        });
        return task;
    }
}