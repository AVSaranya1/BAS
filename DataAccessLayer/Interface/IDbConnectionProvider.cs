using DataAccessLayer.Model;
using System.Data;

namespace DataAccessLayer.Interface;

public interface IDbConnectionProvider
{
    IDbConnection CreateConnection(DatabaseType databaseType);
}
