using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Newtonsoft.Json;
using System.Data;

namespace DataAccessLayer.Implementation;

public class EntityDAL : RepositoryBase, IEntityDAL
{
    public EntityDAL(IDbTransaction _transaction) : base(_transaction)
    { }
    public async Task<List<EntityModel>> GetAllLevelDetail()
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Mode", Common.PageMode.GET);
        var result = await Connection.QueryAsync<EntityModel>("sp_Entity",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }
    public async Task<EntityModel> GetLevelDetailByGuid(string guid)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Mode", Common.PageMode.GET);
        parameters.Add("@Level_Detail_GUID", guid);
        var result = await Connection.QueryFirstOrDefaultAsync<EntityModel>("sp_Entity",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);
        return result;
    }
    public async Task<bool> UpdateLevelDetailAsync(EntityModel model)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@LevelGUID", model.LevelGUID);
        parameters.Add("@Level_Detail_GUID", model.LevelDetailGUID);
        parameters.Add("@Level_Detail_Code", model.LevelDetailCode);
        parameters.Add("@Level_Detail_Name", model.LevelDetailName);
        parameters.Add("@CountryGUID", model.CountryGUID);
        parameters.Add("@TimeZoneGUID", model.TimeZoneGUID);
        parameters.Add("@CurrencyGUID", model.CurrencyGUID);
        parameters.Add("@Logo", model.Logo);
        parameters.Add("@UpdatedBy", model.UpdatedBy);
        parameters.Add("@Active", model.Active);
        parameters.Add("@Latitude", model.Latitude);
        parameters.Add("@Longitude", model.Longitude);
        parameters.Add("@Radius", model.Radius);
        parameters.Add("@ProjectAddress", model.ProjectAddress);
        parameters.Add("@BuilderUEN_No", model.BuilderUEN_No);
        parameters.Add("@ProjectBP_No", model.ProjectBP_No);
        parameters.Add("@ProjectName", model.ProjectName);
        parameters.Add("@BuilderName", model.BuilderName);
        parameters.Add("@ProjectStartDate", model.ProjectStartDate);
        parameters.Add("@ProjectEndDate", model.ProjectEndDate);
        parameters.Add("@SiteCode", model.SiteCode);
        parameters.Add("@Mode", Common.PageMode.EDIT);

        var result = await Connection.ExecuteAsync("sp_Entity",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);

        return result > 0 ? true : false;
    }    
    public async Task<bool> InsertLevelDetailAsync(EntityModel model)
    {
        DynamicParameters parameters = new DynamicParameters();        
        parameters.Add("@LevelGUID", model.LevelGUID);
        parameters.Add("@Level_Detail_Code", model.LevelDetailCode);
        parameters.Add("@Level_Detail_Name", model.LevelDetailName);
        parameters.Add("@CountryGUID", model.CountryGUID);
        parameters.Add("@TimeZoneGUID", model.TimeZoneGUID);
        parameters.Add("@CurrencyGUID", model.CurrencyGUID);
        parameters.Add("@Logo", model.Logo);
        parameters.Add("@UpdatedBy", model.UpdatedBy);
        parameters.Add("@Active", model.Active);
        parameters.Add("@Latitude", model.Latitude);
        parameters.Add("@Longitude", model.Longitude);
        parameters.Add("@Radius", model.Radius);
        parameters.Add("@ProjectAddress", model.ProjectAddress);
        parameters.Add("@BuilderUEN_No", model.BuilderUEN_No);
        parameters.Add("@ProjectBP_No", model.ProjectBP_No);
        parameters.Add("@ProjectName", model.ProjectName);
        parameters.Add("@BuilderName", model.BuilderName);
        parameters.Add("@ProjectStartDate", model.ProjectStartDate);
        parameters.Add("@ProjectEndDate", model.ProjectEndDate);
        parameters.Add("@SiteCode", model.SiteCode);        
        parameters.Add("@Mode", Common.PageMode.ADD);

        var result = await Connection.ExecuteAsync("sp_Entity",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);

        return result > 0;
    }
    public async Task<(bool deleteLevelDetail, List<DeleteLevelDetailResult> deleteResults)> DeleteLevelDetailAsync(DataTable deleteLevelDetailTable)
    {
        if (deleteLevelDetailTable?.Rows.Count > 0)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblDelete", JsonConvert.SerializeObject(deleteLevelDetailTable));
            parameters.Add("@Mode", Common.PageMode.DELETE);

            var result = await Connection.QueryMultipleAsync("sp_Entity",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);

            List<DeleteLevelDetailResult> DeleteData = (await result.ReadAsync<DeleteLevelDetailResult>()).ToList();
            return (DeleteData.Any(), DeleteData.ToList());
        }

        return (false, null);
    }
}
