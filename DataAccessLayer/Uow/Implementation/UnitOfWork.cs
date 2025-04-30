using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Interface;
using System.Data;

namespace DataAccessLayer.Uow.Implementation;

public class UnitOfWork: IUnitOfWork
{
    private readonly IDbConnectionProvider _connectionProvider;
    private IDbConnection _connection;
    private IDbTransaction? _transaction;
    private bool _disposed = false;

    private IAuditLogDAL? _auditLogDALRepo;
    private ILoginDAL? _loginDALRepo;
    private IBusinessEntityDAL? _businessEntityDALRepo;
    private IEntityGroupDAL? _entityGroupDALRepo;
    private IDropdownDAL? _dropdownDALRepo;
    private ILocationDAL? _locationDALRepo;

    public UnitOfWork(IDbConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
        _connection = _connectionProvider.CreateConnection(DatabaseType.Master);
        _connection.Open();
    }

    public void BeginTransaction()
    {
        if (_transaction == null)
        {
            _transaction = _connection.BeginTransaction();
        }
    }

    public Task<int> CompleteAsync()
    {
        if (_transaction is null)
        {
            return Task.FromResult(0);
        }

        try
        {
            _transaction.Commit();
            return Task.FromResult(1);
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void SwitchDatabase(DatabaseType databaseType)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Cannot switch database while a transaction is active.");
        }

        _connection?.Dispose(); // Dispose existing connection
        _connection = _connectionProvider.CreateConnection(databaseType);
        _connection.Open();

        // Reset repositories to use new connection
        ResetRepository();
    }

    private void ResetRepository()
    {
        _auditLogDALRepo = null;
        _loginDALRepo = null;
        _businessEntityDALRepo = null;
        _entityGroupDALRepo = null;
        _dropdownDALRepo = null;
        _locationDALRepo = null;
    }

    public IAuditLogDAL auditLogDALRepo
          => _auditLogDALRepo ??= new AuditLogDAL(_connection, _transaction);

    public ILoginDAL LoginDALRepo
            => _loginDALRepo ??= new LoginDAL(_connection, _transaction);

    public IBusinessEntityDAL BusinessEntityDALRepo
            => _businessEntityDALRepo ??= new BusinessEntityDAL(_connection, _transaction);

    public IEntityGroupDAL EntityGroupDALRepo
            => _entityGroupDALRepo ??= new EntityGroupDAL(_connection, _transaction);

    public IDropdownDAL DropdownDALRepo
            => _dropdownDALRepo ??= new DropdownDAL(_connection, _transaction); 
    
    public ILocationDAL LocationDALRepo
        => _locationDALRepo ??= new LocationDAL(_connection, _transaction);

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            _connection?.Dispose();
            _disposed = true;
        }
    }
}
