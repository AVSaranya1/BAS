using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer.Implementation;
public class DropdownDAL : RepositoryBase, IDropdownDAL
{
    private readonly string _connectionString;

    public DropdownDAL(IDbTransaction _transaction, string connectionString) : base(_transaction)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<DropDownModel>> getLevel(long userID)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserID", userID);
                parameters.Add("@Mode", "LEVEL");

                return await Connection.QueryAsync<DropDownModel>("sp_ListData",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
            }
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
            using (var connection = new SqlConnection(_connectionString))
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
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Mode", "COUNTRY");

                return await Connection.QueryAsync<DropDownModel>("sp_ListData",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
            }
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
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Mode", "CURRENCY");

                return await Connection.QueryAsync<DropDownModel>("sp_ListData",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
            }
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
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Mode", "GET_INDUSTRY_DROPDOWN");

                return await Connection.QueryAsync<DropDownModel>("sp_ListData",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
            }
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

    public async Task<IEnumerable<TimezoneModel>> getTimeZone(long? RefID1)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@RefID1", RefID1);
                parameters.Add("@Mode", "TIMEZONE");

                return await Connection.QueryAsync<TimezoneModel>("sp_ListData",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
            }
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
}
