using System.Data;

namespace DataAccessLayer.Implementation;

public class BaseRepository
{
    public readonly IDbConnection Connection;
    public readonly IDbTransaction Transaction;

    public BaseRepository(IDbConnection connection, IDbTransaction transaction)
    {
        Connection = connection;
        Transaction = transaction;
    }
}
