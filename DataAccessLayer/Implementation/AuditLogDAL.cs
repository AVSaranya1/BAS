using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccessLayer.Implementation
{
    public class AuditLogDAL : BaseRepository, IAuditLogDAL
    {
        public AuditLogDAL(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        { }

        public async Task<bool> LogAudit(AuditLog auditLog)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserId", auditLog.UserId);
                parameters.Add("@UserGuid", auditLog.UserGuid);
                parameters.Add("@Token", auditLog.Token);
                parameters.Add("@Action", auditLog.Action);
                parameters.Add("@IPAddress", auditLog.IPAddress);
                parameters.Add("@DeviceInfo", auditLog.DeviceInfo);
                parameters.Add("@CreatedBy", auditLog.CreatedBy);
                parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

                await Connection.ExecuteAsync(
                    "sp_LogAudit",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure
                );

                int result = parameters.Get<int>("@RetVal");
                string errorMessage = parameters.Get<string>("@ErrorMessage");

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"Audit Log Error: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }
    }
}
