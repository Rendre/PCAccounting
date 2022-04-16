using DB;
using DB.Repositories;
using DB.Repositories.Computers;
using DB.Repositories.Employers;
using DB.Repositories.Files;
using DB.Repositories.Sessions;
using DB.Repositories.Users;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Services;
using SharedKernel.Services.DownloadService;
using SharedKernel.Services.LoginService;
using SharedKernel.Services.SaveService;

namespace SharedKernel;

public static class ServiceCollectionExtations
{
    public static void UseServices(this IServiceCollection collection)
    {
        collection.AddScoped<IFileSave, WebSave>();
        collection.AddScoped<IFileDownload, WebDownload>();
        collection.AddScoped<ILoginService, LoginService>();
    }

    public static void UseEFRepisitories(this IServiceCollection collection)
    {
        collection.AddDbContext<ApplicationContextEF>();

        collection.AddScoped<IComputerRepository, ComputerEFRepository>();
        collection.AddScoped<IEmployerRepository, EmployerEFRepository>();
        collection.AddScoped<IFileRepository, FileEFRepository>();
        collection.AddScoped<ISessionRepository, SessionEFRepository>();
        collection.AddScoped<IUserRepository, UserEFRepository>();

        collection.AddScoped<IUnitOfWork, UnitOfWorkEF>();
    }

    public static void UseDapperRepositories(this IServiceCollection collection)
    {
        collection.AddScoped<MySQLDatabaseContext>();

        collection.AddScoped<IComputerRepository, ComputerDapperRepository>();
        collection.AddScoped<IEmployerRepository, EmployerDapperRepository>();
        collection.AddScoped<IFileRepository, FileDapperRepository>();
        collection.AddScoped<ISessionRepository, SessionDapperRepository>();
        collection.AddScoped<IUserRepository, UserDapperRepository>();

        collection.AddScoped<IUnitOfWork, UnitOfWorkDapper>();
    }
}

