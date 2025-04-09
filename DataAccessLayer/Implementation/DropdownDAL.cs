using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.SqlServer.Management.Smo;
using System.Data;

namespace DataAccessLayer.Implementation;
public class DropdownDAL : RepositoryBase,IDropdownDAL
{
    public DropdownDAL(IDbTransaction _transaction) : base(_transaction)
    {
            
    }
    public async Task<IEnumerable<DropDownModel>> getLevel(long userID)
    {
        DynamicParameters parameters = new DynamicParameters();        
        parameters.Add("@UserID", userID);        
        parameters.Add("@Mode", "LEVEL");

        return await Connection.QueryAsync<DropDownModel>("sp_ListData",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<DropDownModel>> getParentEntity(long userID,string RefID1,string RefID2)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RefID1", RefID1);
        parameters.Add("@RefID2", RefID2);
        parameters.Add("@UserID", userID);       
        parameters.Add("@Mode", "PARENT_ENTITY");

        return await Connection.QueryAsync<DropDownModel>("sp_ListData",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<DropDownModel>> getCountry()
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Mode", "COUNTRY");

        return await Connection.QueryAsync<DropDownModel>("sp_ListData",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<DropDownModel>> getCurrency()
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Mode", "CURRENCY");

        return await Connection.QueryAsync<DropDownModel>("sp_ListData",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<TimezoneModel>> getTimeZone(long? RefID1)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RefID1", RefID1);
        parameters.Add("@Mode", "TIMEZONE");

        return await Connection.QueryAsync<TimezoneModel>("sp_ListData",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<DropDownModel>> getEntityGroup()
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Mode", "Entity_Group");

        return await Connection.QueryAsync<DropDownModel>("sp_ListData",
            parameters,
            transaction: Transaction,
            commandType: CommandType.StoredProcedure);
    }
}
