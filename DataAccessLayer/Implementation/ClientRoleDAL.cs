using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;

namespace DataAccessLayer.Implementation
{
    public class ClientRoleDAL : RepositoryBase, IClientRoleDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClientRoleDAL(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(new SqlConnection(configuration.GetConnectionString("connection")))
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        public async Task<(bool? DeleteRole, List<DeleteClientRoleInformation?> deleteRoleInformation)> DeleteRole(ClientRolesDelete? rolesdelete, long UserId)
        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblDelete", JsonConvert.SerializeObject(rolesdelete?.DeleteRoleTable));
            parameters.Add("@UpdatedBy", UserId);
            parameters.Add("@Mode", Common.PageMode.DELETE);
            var Result = await Connection.QueryMultipleAsync("dbo.sp_ClientRoleUserCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            List<DeleteClientRoleInformation?> DeleteRoleInfo = (await Result.ReadAsync<DeleteClientRoleInformation?>()).ToList();
            while (!Result.IsConsumed)
            {
                await Result.ReadAsync();
            }
            bool? res = DeleteRoleInfo.Any();
            return (res, DeleteRoleInfo);
        }

        public async Task<List<GetClientRoleModel>> GetAllRole(long UpdatedBy, string ClientDBName)
        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleGUID", string.Empty);
            parameters.Add("@Mode", Common.PageMode.GET);
            parameters.Add("@UpdatedBy", UpdatedBy);
            parameters.Add("@ClientDBName", ClientDBName);
            var multi = await Connection.QueryMultipleAsync("dbo.sp_ClientRoleUserCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<GetClientRoleModel>().ToList();
        }

        public async Task<(ClientRoleModel? rolemodel, List<ClientModules?> ModuleDatatable)> getModulesBasedOnRole(string? RoleGUID, long? updatedBy, string? ClientDBName)
        {
          
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleGUID", RoleGUID);
            parameters.Add("@UpdatedBy", updatedBy);
            parameters.Add("@Mode", Common.PageMode.GET_MODULE_INFORMATION);
            parameters.Add("@ClientDBName", ClientDBName);
            var multi = await Connection.QueryMultipleAsync("dbo.sp_ClientRoleUserCreation",
                                                             parameters,
                                                             transaction: Transaction,
                                                             commandType: CommandType.StoredProcedure);
            var Roles = (await multi.ReadAsync<ClientRoleModel?>()).FirstOrDefault();
            var Moduleinfo = (await multi.ReadAsync<ClientModules?>()).ToList();

            return (Roles, Moduleinfo);
        }

        public async Task<(List<ClientRoleModel?> roleModels, long? RetVal, string? Msg)> InsertUpdateRole(ClientRoleModel? model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", model?.RoleId);
            parameters.Add("@RoleDesc", model?.RoleName);
            parameters.Add("@IsAdmin", model?.IsAdmin);
            parameters.Add("@IsEntityAdmin", model?.IsEntityAdmin);
            parameters.Add("@DisplayPDPAData", model?.DisplayPDPAData);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@AccessToAllClient", model?.AccessToAllClient);
            parameters.Add("@LevelDetailGuid", model?.LevelDetailsGuid);
            parameters.Add("@LevelGuid", model?.LevelGuid);
            parameters.Add("@tblRARDetail", JsonConvert.SerializeObject(model?.ModuleTable), DbType.String);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@Mode", Common.PageMode.ADD);
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@ClientDBName", model?.ClientDBName);
            parameters.Add("@RoleGUID", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var result = await Connection.QueryMultipleAsync("dbo.sp_ClientRoleUserCreation",
                                                              parameters,
                                                              transaction: Transaction,
                                                              commandType: CommandType.StoredProcedure);

            var roles = result.Read<ClientRoleModel?>().ToList();

            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            long retVal = parameters.Get<long>("@RetVal");
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            string RoleGuid = parameters.Get<string?>("@RoleGUID") ?? string.Empty;


            // Return the roles list along with output parameters
            return (roles, retVal, msg);
        }


        public async Task<(List<GetClientRoleModel?> roleModels, long? RetVal, string? Msg)> UpdateRole(GetClientRoleModel? model)
        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", model?.RoleId);
            parameters.Add("@RoleDesc", model?.RoleName);
            parameters.Add("@IsAdmin", model?.IsAdmin);
            parameters.Add("@IsEntityAdmin", model?.IsEntityAdmin);
            parameters.Add("@DisplayPDPAData", model?.DisplayPDPAData);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@AccessToAllClient", model?.AccessToAllClient);
            parameters.Add("@ClientDBName", model?.ClientDBName);
            parameters.Add("@LevelDetailID", model?.LevelDetailsID);
            parameters.Add("@LevelID", model?.LevelID);
            parameters.Add("@tblRARDetail", JsonConvert.SerializeObject(model?.ModuleTable), DbType.String);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@Mode", Common.PageMode.EDIT);
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            parameters.Add("@RoleGUID", model?.RoleGuid);
            var result = await Connection.QueryMultipleAsync("dbo.sp_ClientRoleUserCreation",
                                                              parameters,
                                                              transaction: Transaction,
                                                              commandType: CommandType.StoredProcedure);

            var roles = result.Read<GetClientRoleModel?>().ToList();

            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            long retVal = parameters.Get<long>("@RetVal");
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            string RoleGuid = parameters.Get<string?>("@RoleGUID") ?? string.Empty;
            // Return the roles list along with output parameters
            return (roles, retVal, msg);
        }

        public async Task<(List<GetClientRoleModel?> roleModels, long? RetVal, string? Msg)> EditUpdateRoleAsync(GetClientRoleModel model)

        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleID", model.RoleId);
            parameters.Add("@RoleDesc", model.RoleName);
            parameters.Add("@IsAdmin", model.IsAdmin);
            parameters.Add("@IsEntityAdmin", model.IsEntityAdmin);
            parameters.Add("@DisplayPDPAData", model.DisplayPDPAData);
            parameters.Add("@Active", model.Active);
            parameters.Add("@AccessToAllClient", model.AccessToAllClient);
            parameters.Add("@LevelDetailID", model.LevelDetailsID);
            parameters.Add("@LevelID", model.LevelID);
            parameters.Add("@tblRARDetail", JsonConvert.SerializeObject(model.ModuleTable), DbType.String);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@Mode", Common.PageMode.EDIT);
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var result = await Connection.QueryMultipleAsync("dbo.sp_ClientRoleUserCreation",
                                                              parameters,
                                                              transaction: Transaction,
                                                              commandType: CommandType.StoredProcedure);

            var roles = result.Read<GetClientRoleModel?>().ToList();

            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            long retVal = parameters.Get<long>("@RetVal");
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            // Return the roles list along with output parameters
            return (roles, retVal, msg);
        }
    }
}
