using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class EntityGroupDAL : BaseRepository, IEntityGroupDAL
    {
        public EntityGroupDAL(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {}

        public async Task<string> AddEntityGroup(EntityGroupModel entityGroupModel)
        {
            try
            {
                    var parameters = new DynamicParameters();
                  
                    parameters.Add("@Mode", Common.PageMode.ADD);
                    parameters.Add("@EntityGroupCode", entityGroupModel.EntityGroupCode);
                    parameters.Add("@EntityGroupName", entityGroupModel.EntityGroupName);
                    parameters.Add("@EntityGroupDesc", entityGroupModel.EntityGroupDesc);
                    parameters.Add("@Logo", entityGroupModel.Logo);
                    parameters.Add("@IsChild", entityGroupModel.IsChild);
                    parameters.Add("@ParentGuid", entityGroupModel.ParentEntityGroupGuid);
                    //parameters.Add("@ParentID", entityGroupModel.ParentID);
                    parameters.Add("@CreatedBy", entityGroupModel.CreatedBy);
                    parameters.Add("@Msg", dbType: DbType.String, size: 2000, direction: ParameterDirection.Output);

                    await Connection.ExecuteAsync(
                        "sp_EntityGroup",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return parameters.Get<string>("@Msg");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> EditEntityGroup(UpdateEntityGroupModel entityGroupModel)
        {
            try
            {
                    var parameters = new DynamicParameters();

                    parameters.Add("@Mode", entityGroupModel.Mode);
                    parameters.Add("@EntityGroupCode", entityGroupModel.EntityGroupCode);
                    
                    parameters.Add("@EntityGroupName", entityGroupModel.EntityGroupName);
                    parameters.Add("@EntityGroupDesc", entityGroupModel.EntityGroupDesc);
                    parameters.Add("@Logo", entityGroupModel.Logo);
                    parameters.Add("@IsChild", entityGroupModel.IsChild);
                    parameters.Add("@ParentGuid", entityGroupModel.ParentEntityGroupGuid);
                    //parameters.Add("@ParentID", entityGroupModel.ParentID);
                    parameters.Add("@Guid", entityGroupModel.Guid);
                    parameters.Add("@CreatedBy", entityGroupModel.CreatedBy);
                    parameters.Add("@Msg", dbType: DbType.String, size: 2000, direction: ParameterDirection.Output);

                    await Connection.ExecuteAsync(
                        "sp_EntityGroup",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return parameters.Get<string>("@Msg");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public async Task<List<DeleteResultModel>> DeleteEntityGroup(List<EntityGroupDel> lstEntityGroupDel, string strMode,string strUserGuid)
        {
            try
            {
                    var parameters = new DynamicParameters();

                    // Table-Valued Parameter (TVP)
                    var table = new DataTable();
                    table.Columns.Add("ID", typeof(long));
                    table.Columns.Add("Guid", typeof(Guid));

                    if (lstEntityGroupDel != null)
                    {
                        foreach (var item in lstEntityGroupDel)
                        {
                            Guid parsedGuid = Guid.Empty;

                            // Convert the nullable Guid to string first
                            var guidStr = item.Guid?.ToString();

                            if (!string.IsNullOrWhiteSpace(guidStr))
                            {
                                Guid.TryParse(guidStr, out parsedGuid);
                            }

                            table.Rows.Add(item.ID, parsedGuid);
                        }
                    }

                    // Ensure you pass the correct table type name
                    parameters.Add("@dtDeleteId", table.AsTableValuedParameter("dbo.utt_DeleteGuid"));
                    parameters.Add("@Mode", "DELETE");
                    parameters.Add("@ModifiedBy", strUserGuid);
                    parameters.Add("@Msg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

                    var result = await Connection.QueryAsync<DeleteResultModel>(
                        "sp_EntityGroup",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.ToList();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return new List<DeleteResultModel> { new DeleteResultModel { SNo = 0, LevelInfo = "Error", Result = "Failed", Remarks = "SQL Error occurred." } };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<DeleteResultModel> { new DeleteResultModel { SNo = 0, LevelInfo = "Error", Result = "Failed", Remarks = "An unexpected error occurred." } };
            }
        }

        public async Task<IEnumerable<DropDownModel>> GetMapEntityGroup()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Msg", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("@Mode", "GET_MAP_ENTITY_GROUP");

            return await Connection.QueryAsync<DropDownModel>("sp_EntityGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<GetEntityGroupModel>> GetEntityGroup(GetEntityGroupModel entityGroupModel)
        {
            List<GetEntityGroupModel> lstResult = new List<GetEntityGroupModel>();
            try
            {

                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@Mode", Common.PageMode.GET);
                    parameters.Add("@Msg", dbType: DbType.String, size: 2000, direction: ParameterDirection.Output);

                    var multi = await Connection.QueryMultipleAsync(
                        "sp_EntityGroup",
                        parameters,
                        transaction: Transaction,
                        commandType: CommandType.StoredProcedure);

                    return multi.Read<GetEntityGroupModel>().ToList();
                }
                catch
                {
                    throw;
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

        public async Task<List<GetEntityGroupModel>> GetEntityGroupDetails(GetEntityGroupModel entityGroupModel)
        {
            List<GetEntityGroupModel> lstResult = new List<GetEntityGroupModel>();
            try
            {
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@Mode", Common.PageMode.GET);
                    parameters.Add("@Guid", entityGroupModel.Guid);
                    parameters.Add("@Msg", dbType: DbType.String, size: 2000, direction: ParameterDirection.Output);

                    var multi = await Connection.QueryMultipleAsync(
                        "sp_EntityGroup",
                        parameters,
                        transaction: Transaction,
                        commandType: CommandType.StoredProcedure);

                    return multi.Read<GetEntityGroupModel>().ToList();
                }
                catch
                {
                    throw;
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
