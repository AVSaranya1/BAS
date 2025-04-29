using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Model.BusinessEntity;
using Newtonsoft.Json;
using System.Data;

namespace DataAccessLayer.Implementation
{
    public class CostCenterDAL : RepositoryBase, ICostCenterDAL
    {
        private readonly string _connectionString;
        public CostCenterDAL(IDbTransaction? _transaction, string connectionString) : base(_transaction)
        {
            _connectionString = connectionString;
        }

        public async Task<string> AddCostCenterAsync(CostCenterModel model, DataTable dataTable)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@GUID", Guid.NewGuid());
            parameters.Add("@dtCostCenter", dataTable.AsTableValuedParameter("utt_CostCenter"));
            parameters.Add("@CostCenterCode", model.CostCenterCode);
            parameters.Add("@CostCenterDesc", model.CostCenterDesc);
            parameters.Add("@IsChild", model.IsChild);
            parameters.Add("@Active", 1);
            parameters.Add("@ParentCostCenterGuid", model.ParentCostCenterGuID);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("@Mode", Common.PageMode.ADD);

            var result = await Connection.ExecuteAsync("sp_CostCenter",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);

            return parameters.Get<string>("@ReturnValue");
        }

        public async Task<string> DeleteCostCenterAsync(DataTable deleteLevelDetailTable, string? ModifiedBy)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@dtDelete", deleteLevelDetailTable.AsTableValuedParameter("utt_DeleteByGUID"));
            parameters.Add("@ModifiedBy",ModifiedBy);
            parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("@Mode", Common.PageMode.DELETE);

            var result = await Connection.QueryMultipleAsync("sp_CostCenter",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<string>("@ReturnValue");
        }

        public async Task<IEnumerable<GetCostCenterModel>> GetAllCostCenter()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("@Mode", Common.PageMode.GET);

            return await Connection.QueryAsync<GetCostCenterModel>("sp_CostCenter",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<(GetCostCenterModel getCostCenterModel, List<MultiSelectionDropDownModel> getBusinessEntityTables,List<MultiSelectionDropDownModel>? getDivisionDatatables, List<MultiSelectionDropDownModel?> getDepartmentDatatables)> GetCostCenterByGuId(string GuId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Guid", Guid.Parse(GuId));
            parameters.Add("@Mode", Common.PageMode.GET_DETAIL);

            var multi = await Connection.QueryMultipleAsync("sp_CostCenter",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var result = multi.Read<GetCostCenterModel>().FirstOrDefault();
            var businessEntityDatatables = (await multi.ReadAsync<MultiSelectionDropDownModel>())?.ToList();
            var Divisiontable = (await multi.ReadAsync<MultiSelectionDropDownModel>())?.ToList();
            var DeptTable = (await multi.ReadAsync<MultiSelectionDropDownModel>())?.ToList();
            return (result, businessEntityDatatables, Divisiontable,DeptTable);
        }
        public async Task<(List<DropDownModel?> getBusinessEntityTables, List<DropDownModel?> getDivisionDatatables, List<DropDownModel?> getDepartmentDatatables)> getMapBUDivisionDept()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Guid", null);
            parameters.Add("@Mode", Common.PageMode.GET_DETAIL);

            var multi = await Connection.QueryMultipleAsync("sp_CostCenter",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            
            var businessEntityDatatables = (await multi.ReadAsync<DropDownModel?>()).ToList();
            var Divisiontable = (await multi.ReadAsync<DropDownModel?>()).ToList();
            var DeptTable = (await multi.ReadAsync<DropDownModel?>()).ToList();
            return (businessEntityDatatables, Divisiontable, DeptTable);
        }
        public async Task<IEnumerable<DropDownModel>> getMapParentCostCenter()
        {
            DynamicParameters parameters = new DynamicParameters();
            
            parameters.Add("@Mode", Common.PageMode.GET_MAP_PARENT_COSTCENTER);

            return await Connection.QueryAsync<DropDownModel>("sp_CostCenter",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
        }
        public async Task<string> UpdateCostCenterAsync(UpdateCostCenterModel model, DataTable dataTable)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@GUID", model.GUID);
            parameters.Add("@dtCostCenter", dataTable.AsTableValuedParameter("utt_CostCenter"));
            parameters.Add("@CostCenterCode", model.CostCenterCode);
            parameters.Add("@CostCenterDesc", model.CostCenterDesc);
            parameters.Add("@IsChild", model.IsChild);
            parameters.Add("@Active", model.Active);
            parameters.Add("@ParentCostCenterGuid", model.ParentCostCenterGuID);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("@Mode", Common.PageMode.EDIT);

            var result = await Connection.ExecuteAsync("sp_CostCenter",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);

            return parameters.Get<string>("@ReturnValue");
        }
    }
}
