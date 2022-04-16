using DB.Repositories.Computers;
using DB.Repositories.Employers;
using DB.Repositories.Files;
using DB.Repositories.Sessions;
using DB.Repositories.Users;

namespace DB.Repositories;

public interface IUnitOfWork : IDisposable
{
    public IComputerRepository ComputerRepository { get; }

    public IEmployerRepository EmployeeRepository { get; }

    public IFileRepository FileRepository { get; }

    public ISessionRepository SessionRepository { get; }

    public IUserRepository UserRepository { get; }
}