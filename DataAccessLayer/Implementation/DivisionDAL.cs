

using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Newtonsoft.Json;
using System;
using System.Data;

namespace DataAccessLayer.Implementation
{
    public class DivisionDAL : RepositoryBase, IDivisionDAL
    {
        public DivisionDAL(IDbTransaction _transaction) : base(_transaction)
        {

        }
        public async Task<(bool deleteClientDivision, List<DeleteDivisionResult> deleteResults)> DeleteClientDivision(long id, DeleteDivision deleteClientDivision)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblDivision", deleteClientDivision.DeleteDivisionDataTable.AsTableValuedParameter("utt_DeleteByGuid"));
            parameters.Add("@UpdatedBy", id);
            parameters.Add("@Mode", Common.PageMode.DELETE);
            var Result = await Connection.QueryMultipleAsync("sp_Division",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            List<DeleteDivisionResult> DeleteClientDivision = (await Result.ReadAsync<DeleteDivisionResult>()).ToList();
            while (!Result.IsConsumed)
            {
                await Result.ReadAsync();
            }

            bool res = DeleteClientDivision.Any();
            return (res, DeleteClientDivision.ToList());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetDivisionModel>> GetAllClientDivision(GetDivisionModel getClientDivisionModel)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@DivisionGUId", string.IsNullOrWhiteSpace(getClientDivisionModel.DivisionGuid.ToString())
                ? null: getClientDivisionModel.DivisionGuid);
            parameters.Add("@TimeZoneID", getClientDivisionModel.TimeZoneID);
            parameters.Add("@UpdatedBy", getClientDivisionModel.CreatedBy);
            parameters.Add("@Mode", Common.PageMode.GET);
            var multi = await Connection.QueryMultipleAsync("sp_Division",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<GetDivisionModel>().ToList();
        }

        public async Task<(GetDivisionModel? getClientDivisionModel,  List<DropDownModel?> getBusinessEntityDatatables, List<DropDownModel>? getCostCenterDivisionDatatables, List<MultiSelectionDropDownModel?> getDepartmentDatatables)> GetClientDivisionByGUId(string GUId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@DivisionGUId", Guid.Parse(GUId));
            parameters.Add("@Mode", Common.PageMode.GET_DETAIL);
            var multi = await Connection.QueryMultipleAsync("sp_Division",
                                                            parameters,
                                                            transaction: Transaction,
                                                            commandType: CommandType.StoredProcedure);
            var res = multi.Read<GetDivisionModel>().FirstOrDefault();
            var businessEntityDatatables = (await multi.ReadAsync<DropDownModel>())?.ToList();
            var CostCenterTable = (await multi.ReadAsync<DropDownModel>())?.ToList();
            var DeptTable = (await multi.ReadAsync<MultiSelectionDropDownModel>())?.ToList();

            return (res, businessEntityDatatables, CostCenterTable, DeptTable);
        }

        public async Task<(List<DropDownModel?> getBusinessEntityDatatables, List<DropDownModel?> getCostCenterDivisionDatatables, List<DropDownModel?> getDepartmentDatatables)> GetClientDivisionDeptCatMap()
        {
            DynamicParameters parameters = new DynamicParameters();
            
            parameters.Add("@DivisionGUId", null);
            parameters.Add("@Mode", Common.PageMode.GET_DETAIL);
            var multi = await Connection.QueryMultipleAsync("sp_Division",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var BusinessEntityTable = (await multi.ReadAsync<DropDownModel>())?.ToList();
            var CostCentertable = (await multi.ReadAsync<DropDownModel>())?.ToList();
            var Departmenttable = (await multi.ReadAsync<DropDownModel>())?.ToList();
            return (BusinessEntityTable, CostCentertable, Departmenttable);
        }

        public async Task<List<DropDownModel?> > getParentDivisionMap()
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Mode", Common.PageMode.GET_MAP_PARENT_DIVISION);
            var multi = await Connection.QueryMultipleAsync("sp_Division",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            
            return multi.Read<DropDownModel>().ToList(); 
        }


        public async Task<(bool InsertClientDivision, long RetVal, string Msg)> InsertUpdateClientDivision(DivisionModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Division_Code", model.Division_Code);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@Division_Desc", model.Division_Desc);
            parameters.Add("@Active", 1);
            parameters.Add("@IsChild", model.IsChild);
            parameters.Add("@ParentDivisionGuid", model.ParentDivisionGuid);
            parameters.Add("@Mode", Common.PageMode.ADD);
            parameters.Add("@tblDivisionDetails", model?.DepartmentCatTable.AsTableValuedParameter("utt_DivisionBUMapping"));
            parameters.Add("@DivisionGuid", model?.DivisionGuid);

            // Declare output parameters explicitly
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            var multi = await Connection.QueryMultipleAsync("sp_Division",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
            var divisionModels = (await multi.ReadAsync<DivisionModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = divisionModels.Any();
            long RetVal = parameters.Get<long>("@RetVal");
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            return (res, RetVal, Msg);
        }

        public async Task<(bool UpdateClientDivision, long RetVal, string Msg)> UpdateClientDivision(UpdateDivision model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Division_Code", model.Division_Code);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@Division_Desc", model.Division_Desc);
            parameters.Add("@DivisionGuid", model.DivisionGuid);
            parameters.Add("@Active", model.Active);
            parameters.Add("@IsChild", model.IsChild);
            parameters.Add("@ParentDivisionGuid", model.ParentDivisionGuid);
            parameters.Add("@Mode", Common.PageMode.EDIT);
            parameters.Add("@tblDivisionDetails", model?.DepartmentCatTable.AsTableValuedParameter("utt_DivisionBUMapping"));
            
            // Declare output parameters explicitly
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_Division",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var divisions = (await multi.ReadAsync<UpdateDivision>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = divisions.Any();
            long RetVal = parameters.Get<long>("@RetVal");
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            return (res, RetVal, Msg);
        }
    }
}
