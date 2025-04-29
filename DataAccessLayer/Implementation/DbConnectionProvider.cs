using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Data;

namespace DataAccessLayer.Implementation;

public class DbConnectionProvider: IDbConnectionProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _masterConnectionString;    
    private readonly EncryptedDecrypt _encryptedDecrypt;
    private static readonly ConcurrentDictionary<Guid, string> _orgDatabaseCache = new();

    public DbConnectionProvider(IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _encryptedDecrypt = new EncryptedDecrypt(config);       
        _httpContextAccessor = httpContextAccessor;
        _masterConnectionString = _encryptedDecrypt.Decrypt(config?.GetConnectionString("connectionEnc"));
    }

    protected Guid GetOrgId()
    {
        if (Guid.TryParse(_httpContextAccessor.HttpContext?.Items["OrgId"]?.ToString(), out var orgId))
        {
            return orgId;
        }
        throw new InvalidOperationException("Organization ID not found in request context.");
    }

    public IDbConnection CreateConnection(DatabaseType databaseType)
    {
        if (databaseType == DatabaseType.Master)
        {
            return new SqlConnection(_masterConnectionString);
        }

        return GetOrgConnection();
    }

    private IDbConnection GetOrgConnection()
    {
        Guid orgId = GetOrgId();

        if (!_orgDatabaseCache.TryGetValue(orgId, out var connectionString))
        {
            connectionString = CreateOrgConnection(orgId);
            _orgDatabaseCache.TryAdd(orgId, connectionString);
        }
        return new SqlConnection(connectionString);
    }

    private string CreateOrgConnection(Guid orgId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@OrgGUID", orgId);
        parameters.Add("@Mode", "GET_DB");

        using var connection = new SqlConnection(_masterConnectionString);
        var db = connection.QueryFirstOrDefault<OrgDbConnectionModel>("sp_Authentication_Conn",
            parameters,
            commandType: CommandType.StoredProcedure);

        if (db is null)
        {
            throw new InvalidOperationException($"No db detail found for org: {orgId}");
        }
        return BuildConnectionString(
            _encryptedDecrypt.Decrypt(db.InstanceName),
            _encryptedDecrypt.Decrypt(db.UserName),
            _encryptedDecrypt.Decrypt(db.Password),
            db.DBName);
    }

    private string BuildConnectionString(string? serverName, string? userID, string? password, string? dbName)
    {
        var builder = new SqlConnectionStringBuilder(_masterConnectionString)
        {
            DataSource = serverName,
            UserID = userID,
            Password = password,
            InitialCatalog = dbName,
            TrustServerCertificate = true,
            MultipleActiveResultSets = true
        };
        return builder.ToString();
    }
}
