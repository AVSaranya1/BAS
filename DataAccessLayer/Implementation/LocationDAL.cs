using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System.Data;

namespace DataAccessLayer.Implementation;

public class LocationDAL: BaseRepository, ILocationDAL
{
    public LocationDAL(IDbConnection connection, IDbTransaction transaction): base(connection, transaction)
    { }

    //public async Task<IEnumerable<LocationModel>> GetLocationsAsync()
    //{
    //    DynamicParameters parameters = new DynamicParameters();
    //    parameters.Add("@Mode", Common.PageMode.GET);

    //    return await Connection.QueryAsync<LocationModel>("sp_Location",
    //        parameters,
    //        commandType: CommandType.StoredProcedure);
    //}

    //public async Task<LocationModel> GetLocationByIdAsync(Guid locationGUID)
    //{
    //    DynamicParameters parameters = new DynamicParameters();
    //    parameters.Add("@GUID", locationGUID);
    //    parameters.Add("@Mode", Common.PageMode.GET_DETAIL);

    //    var result = await Connection.QueryFirstOrDefaultAsync<LocationModel>("sp_Location",
    //        parameters,
    //        commandType: CommandType.StoredProcedure);
    //}

    //public async Task<int> AddLocationAsync(LocationModel locationModel)
    //{
    //    DynamicParameters parameters = new DynamicParameters();
    //    parameters.Add("@LocationCode", locationModel.LocationCode);
    //    parameters.Add("@LocationDesc", locationModel.LocationDesc);
            
    //    return await Connection.ExecuteAsync("sp_Location",
    //        parameters,
    //        Transaction,
    //        commandType: CommandType.StoredProcedure);
    //}

    //public async Task<int> UpdateLocationAsync(LocationModel locationModel)
    //{
    //    DynamicParameters parameters = new DynamicParameters();
    //    parameters.Add("@GUID", locationModel.GUID);
    //    parameters.Add("@LocationCode", locationModel.LocationCode);

    //    return await Connection.ExecuteAsync("sp_Location",
    //        parameters,
    //        Transaction,
    //        commandType: CommandType.StoredProcedure);
    //}

    //public async Task<int> DeleteLocationAsync(Guid locationGUID)
    //{
    //    DynamicParameters parameters = new DynamicParameters();
    //    parameters.Add("@GUID", locationGUID);
    //    parameters.Add("@Mode", Common.PageMode.DELETE);

    //    return await Connection.ExecuteAsync("sp_Location",
    //        parameters,
    //        Transaction,
    //        commandType: CommandType.StoredProcedure);
    //}
}
