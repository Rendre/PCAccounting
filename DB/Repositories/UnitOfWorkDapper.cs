using DB.Repositories.Computers;
using DB.Repositories.Employers;
using DB.Repositories.Files;
using DB.Repositories.Sessions;
using DB.Repositories.Users;

namespace DB.Repositories;

public class UnitOfWorkDapper : IUnitOfWork
{
    private readonly MySQLDatabaseContext _db;
    private IComputerRepository? _computerRepository;
    private IEmployerRepository? _employerRepository;
    private IFileRepository? _fileRepository;
    private ISessionRepository? _sessionRepository;
    private IUserRepository? _userRepository;
    private bool _disposed;

    public UnitOfWorkDapper(MySQLDatabaseContext db)
    {
        _db = db;
    }

    public IComputerRepository ComputerRepository
    {
        get { return _computerRepository ??= new ComputerDapperRepository(_db); }
    }

    public IEmployerRepository EmployeeRepository
    {
        get { return _employerRepository ??= new EmployerDapperRepository(_db); }
    }

    public IFileRepository FileRepository
    {
        get { return _fileRepository ??= new FileDapperRepository(_db); }
    }

    public ISessionRepository SessionRepository
    {
        get { return _sessionRepository ??= new SessionDapperRepository(_db); }
    }

    public IUserRepository UserRepository
    {
        get { return _userRepository ??= new UserDapperRepository(_db); }
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