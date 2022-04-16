using DB.Repositories.Computers;
using DB.Repositories.Employers;
using DB.Repositories.Files;
using DB.Repositories.Sessions;
using DB.Repositories.Users;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories;

public class UnitOfWorkEF : IUnitOfWork
{
    private readonly ApplicationContextEF _db;
    private IComputerRepository? _computerRepository;
    private IEmployerRepository? _employerRepository;
    private IFileRepository? _fileRepository;
    private ISessionRepository? _sessionRepository;
    private IUserRepository? _userRepository;
    private bool _disposed;

    public UnitOfWorkEF(ApplicationContextEF db)
    {
        _db = db;
    }

    public IComputerRepository ComputerRepository
    {
        get { return _computerRepository ??= new ComputerEFRepository(_db); }
    }

    public IEmployerRepository EmployeeRepository
    {
        get { return _employerRepository ??= new EmployerEFRepository(_db); }
    }

    public IFileRepository FileRepository
    {
        get { return _fileRepository ??= new FileEFRepository(_db); }
    }

    public ISessionRepository SessionRepository
    {
        get { return _sessionRepository ??= new SessionEFRepository(_db); }
    }

    public IUserRepository UserRepository
    {
        get { return _userRepository ??= new UserEFRepository(_db); }
    }

    public virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _db.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}