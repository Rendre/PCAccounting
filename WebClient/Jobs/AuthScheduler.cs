using Quartz;
using Quartz.Impl;

namespace WebClient.Jobs;

public class AuthScheduler
{
    public static async void Start()
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        var job = JobBuilder.Create<AuthCleaner>().Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("authTrigger", "group")
            .StartNow()
            .WithSimpleSchedule(e => e.WithIntervalInSeconds(10)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}