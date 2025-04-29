using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Model.BusinessEntity;
using System.Data;

namespace DataAccessLayer.Implementation;

public class BusinessEntityDAL : BaseRepository, IBusinessEntityDAL
{
    public BusinessEntityDAL(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
    { }

    public async Task<IEnumerable<BusinessEntityModel>> GetAllBusinessEntity()
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        parameters.Add("@Mode", Common.PageMode.GET);

        return await Connection.QueryAsync<BusinessEntityModel>("sp_BusinessEntity",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
    public async Task<BusinessEntityModel> GetBusinessEntityByGuiD(Guid guid)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@GUID", guid);
        parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        parameters.Add("@Mode", Common.PageMode.GET_DETAIL);
        
        var result = await Connection.QueryFirstOrDefaultAsync<BusinessEntityModel>("sp_BusinessEntity",
            parameters,
            commandType: CommandType.StoredProcedure);
        return result;
    }
    public async Task<string> UpdateBusinessEntityAsync(UpdateBusinessEntityModel model)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@GUID", model.Guid);
        parameters.Add("@BusinessEntityCode", model.BusinessEntityCode);
        parameters.Add("@BusinessEntityDesc", model.BusinessEntityDesc);
        parameters.Add("@Logo", model.Logo);
        parameters.Add("@Address1", model.Address1);
        parameters.Add("@Address2", model.Address2);
        parameters.Add("@PostalCode", model.PostalCode);
        parameters.Add("@CountryGUID", model.CountryGUID);
        parameters.Add("@PrimaryContactName", model.PrimaryContactName);
        parameters.Add("@PrimaryContactDesignation", model.PrimaryContactDesignation);
        parameters.Add("@PrimaryContactNumber", model.PrimaryContactNumber);
        parameters.Add("@PrimaryContactEmail", model.PrimaryContactEmail);
        parameters.Add("@SecondaryContactName", model.SecondaryContactName);
        parameters.Add("@SecondaryContactDesignation", model.SecondaryContactDesignation);
        parameters.Add("@SecondaryContactNumber", model.SecondaryContactNumber);
        parameters.Add("@SecondaryContactEmail", model.SecondaryContactEmail);
        parameters.Add("@BillingAddress1", model.BillingAddress1);
        parameters.Add("@BillingAddress2", model.BillingAddress2);
        parameters.Add("@BillingPostalCode", model.BillingPostalCode);
        parameters.Add("@BillingCountryGUID", model.BillingCountryGUID);
        parameters.Add("@TimeZoneGUID", model.TimeZoneGUID);
        parameters.Add("@CurrencyGUID", model.CurrencyGUID);
        parameters.Add("@LatitudeCoordinate", model.LatitudeCoordinate);
        parameters.Add("@LongitudeCoordinate", model.LongitudeCoordinate);
        parameters.Add("@Radius", model.Radius);
        parameters.Add("@Account_Code", model.Account_Code);
        parameters.Add("@Integration_Code", model.Integration_Code);
        parameters.Add("@IsChild", model.IsChild);
        parameters.Add("@ParentBusinessEntityGUID", model.ParentBusinessEntityGUID);
        parameters.Add("@EntityGroupGUID", model.EntityGroupGUID);
        parameters.Add("@Active", model.Active);
        parameters.Add("@ModifiedBy", model.ModifiedBy);
        parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        parameters.Add("@Mode", Common.PageMode.EDIT);

        var result = await Connection.ExecuteAsync("sp_BusinessEntity",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<string>("@ReturnValue");
    }    
    public async Task<string> AddBusinessEntityAsync(AddBusinessEntityModel model)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@GUID", Guid.NewGuid());
        parameters.Add("@BusinessEntityCode", model.BusinessEntityCode);
        parameters.Add("@BusinessEntityDesc", model.BusinessEntityDesc);
        parameters.Add("@Logo", model.Logo);
        parameters.Add("@Address1", model.Address1);
        parameters.Add("@Address2", model.Address2);
        parameters.Add("@PostalCode", model.PostalCode);
        parameters.Add("@CountryGUID", model.CountryGUID);
        parameters.Add("@PrimaryContactName", model.PrimaryContactName);
        parameters.Add("@PrimaryContactDesignation", model.PrimaryContactDesignation);
        parameters.Add("@PrimaryContactNumber", model.PrimaryContactNumber);
        parameters.Add("@PrimaryContactEmail", model.PrimaryContactEmail);
        parameters.Add("@SecondaryContactName", model.SecondaryContactName);
        parameters.Add("@SecondaryContactDesignation", model.SecondaryContactDesignation);
        parameters.Add("@SecondaryContactNumber", model.SecondaryContactNumber);
        parameters.Add("@SecondaryContactEmail", model.SecondaryContactEmail);
        parameters.Add("@BillingAddress1", model.BillingAddress1);
        parameters.Add("@BillingAddress2", model.BillingAddress2);
        parameters.Add("@BillingPostalCode", model.BillingPostalCode);
        parameters.Add("@BillingCountryGUID", model.BillingCountryGUID);
        parameters.Add("@TimeZoneGUID", model.TimeZoneGUID);
        parameters.Add("@CurrencyGUID", model.CurrencyGUID);
        parameters.Add("@LatitudeCoordinate", model.LatitudeCoordinate);
        parameters.Add("@LongitudeCoordinate", model.LongitudeCoordinate);
        parameters.Add("@Radius", model.Radius);
        parameters.Add("@Account_Code", model.Account_Code);
        parameters.Add("@Integration_Code", model.Integration_Code);
        parameters.Add("@IsChild", model.IsChild);
        parameters.Add("@ParentBusinessEntityGUID", model.ParentBusinessEntityGUID);
        parameters.Add("@EntityGroupGUID", model.EntityGroupGUID);
        parameters.Add("@Active", model.Active);
        parameters.Add("@CreatedBy", model.CreatedBy);
        parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        parameters.Add("@Mode", Common.PageMode.ADD);

        var result = await Connection.ExecuteAsync("sp_BusinessEntity",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);

        return parameters.Get<string>("@ReturnValue");
    }
    public async Task<string> DeleteBusinessEntityAsync(DataTable deleteData)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@dtBusinessEntity", deleteData.AsTableValuedParameter("utt_DeleteByGUID"));
        parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        parameters.Add("@Mode", Common.PageMode.DELETE);

        var result = await Connection.QueryMultipleAsync("sp_BusinessEntity",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<string>("@ReturnValue");
    }

    public async Task<IEnumerable<DropDownModel>> GetMapParentBusinessUnit()
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        parameters.Add("@Mode", "GET_MAP_PARENT_BU");

        return await Connection.QueryAsync<DropDownModel>("sp_BusinessEntity",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<DropDownModel>> GetMapEntityGroup()
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReturnValue", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        parameters.Add("@Mode", "GET_MAP_ENTITY_GROUP");

        return await Connection.QueryAsync<DropDownModel>("sp_BusinessEntity",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
}
