using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccessLayer.Implementation
{
    public class EntityMenuDAL : RepositoryBase, IEntityMenuDAL
    {
        private readonly string _connectionString; 

        public EntityMenuDAL(IDbTransaction transaction, string connectionString) : base(transaction)
        {
            _connectionString = connectionString;
        }

        public async Task<List<EntityMenuModel>> GetEntityMenu(string? strMode, string? struserGuid)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Mode", strMode);
                    parameters.Add("@UpdatedBy", struserGuid);

                    var result = await connection.QueryAsync<EntityMenuModel>(
                        "sp_GetEntityLevel",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.ToList(); // Convert IEnumerable to List
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching department data", ex);
            }
        }

        public async Task<List<MenusModel>> GetMenu(string UserNameGuid)
        {
            List<MenusModel> lstResult = new List<MenusModel>();
            try
            {

                using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@UserNameGuid", UserNameGuid);
                           // parameters.Add("@ClientCode", ClientCode);
                            //parameters.Add("@OrgID", OrgID);

                            var multi = await connection.QueryMultipleAsync(
                                "sp_GetMenu",
                                parameters,
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure);

                            //transaction.Commit();

                            return multi.Read<MenusModel>().ToList();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return lstResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return lstResult;
            }
        }
    }
}
