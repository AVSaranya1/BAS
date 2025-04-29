using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer.Implementation;
public class DropdownDAL : BaseRepository, IDropdownDAL
{
    public DropdownDAL(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
    { }

    public async Task<IEnumerable<DropDownModel>> getLevel(long userID)
    {
        try
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserID", userID);
            parameters.Add("@Mode", "LEVEL");

            return await Connection.QueryAsync<DropDownModel>("sp_ListData",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return Enumerable.Empty<DropDownModel>();
    }

    public async Task<IEnumerable<DropDownModel>> getParentEntity(long userID, string RefID1, string RefID2)
    {
        try
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RefID1", RefID1);
            parameters.Add("@RefID2", RefID2);
            parameters.Add("@UserID", userID);
            parameters.Add("@Mode", "PARENT_ENTITY");

            return await Connection.QueryAsync<DropDownModel>("sp_ListData",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return Enumerable.Empty<DropDownModel>();
    }

    public async Task<IEnumerable<DropDownModel>> getIndustry()
    {
        try
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET_INDUSTRY_DROPDOWN");

            return await Connection.QueryAsync<DropDownModel>("sp_ListData",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return Enumerable.Empty<DropDownModel>();
    }

    public async Task<IEnumerable<DropDownModel>> getCurrency()
    {
        try
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", Common.DropdownListType.CURRENCY);

            return await Connection.QueryAsync<DropDownModel>("sp_ListData",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return Enumerable.Empty<DropDownModel>();
    }

    public async Task<IEnumerable<DropDownModel>> getCountry()
    {
        try
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", Common.DropdownListType.COUNTRY);

            return await Connection.QueryAsync<DropDownModel>("sp_ListData",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return Enumerable.Empty<DropDownModel>();
    }

    public async Task<IEnumerable<TimezoneModel>> getTimeZone()
    {
        try
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", Common.DropdownListType.TIMEZONE);

            return await Connection.QueryAsync<TimezoneModel>("sp_ListData",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return Enumerable.Empty<TimezoneModel>();
    }

    public async Task<IEnumerable<DropDownModel>> getMarital()
    {
        try
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", Common.DropdownListType.MARITAL);

            return await Connection.QueryAsync<DropDownModel>("sp_ListData",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return Enumerable.Empty<DropDownModel>();
    }
}
