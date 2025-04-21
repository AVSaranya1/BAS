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
    public class DepartmentDAL : RepositoryBase,IDepartmentDAL
    {
        private readonly string _connectionString;
        public DepartmentDAL(IDbTransaction transaction, string connectionString) : base(transaction)
        {
            _connectionString = connectionString;
        }

        public async Task<GetDept> GetDepartment(GetDepartmentInput departmentInput)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Mode", departmentInput.Mode);
                    parameters.Add("@UpdatedBy", departmentInput.UpdatedGuidBy);
                    parameters.Add("@DeptGUID", departmentInput.DeptGUID);

                    using (var multi = await connection.QueryMultipleAsync(
                        "sp_Department", parameters, commandType: CommandType.StoredProcedure))
                    {

                        //var departmentModel = new GetDept
                        //{
                        //    lstDeptDetails = (await multi.ReadAsync<Dept>()).ToList(),
                        //    lstdeptDetails = (await multi.ReadAsync<DepartmentDetaills>()).ToList()
                        //};

                        var departmentModel = new GetDept
                        {
                            lstDeptDetails = (await multi.ReadAsync<Dept>()).ToList()
                        };

                        if (!multi.IsConsumed)
                            departmentModel.lstdeptDetails = (await multi.ReadAsync<DepartmentDetaills>()).ToList();

                        return departmentModel;
                    }
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException("An error occurred while fetching department data", ex);
            }
        }

        public async Task<GetDeptView> ViewDepartment(GetDepartmentInput departmentInput)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Mode", departmentInput.Mode);
                    parameters.Add("@UpdatedBy", departmentInput.UpdatedGuidBy);
                    parameters.Add("@DeptGUID", departmentInput.DeptGUID);

                    using (var multi = await connection.QueryMultipleAsync(
                        "sp_Department", parameters, commandType: CommandType.StoredProcedure))
                    {
                        //var getDeptView = new GetDeptView
                        //{
                        //    lstDeptDetails = (await multi.ReadAsync<Dept>()).ToList(),
                        //    lstdeptDetails = (await multi.ReadAsync<DepartmentDetaills>()).ToList()
                        //};

                        var departmentModel = new GetDeptView
                        {
                            lstDeptview = (await multi.ReadAsync<DeptView>()).ToList()
                        };

                        if (!multi.IsConsumed)
                            departmentModel.lstdeptDetailsview = (await multi.ReadAsync<DepartmentDetaillsView>()).ToList();


                        return departmentModel;
                    }
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException("An error occurred while fetching department data", ex);
            }
        }

        public async Task<string> InsertDepartmentDetails(AddDept addDept,string struserGuid)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var table = new DataTable();
                    table.Columns.Add("EntityGroupID", typeof(long));
                    table.Columns.Add("BusinessUnitID", typeof(long));
                    table.Columns.Add("CostCenterID", typeof(long));
                    table.Columns.Add("DeptID", typeof(long));
                    table.Columns.Add("DivisionID", typeof(long));
                    foreach (var item in addDept.lstDepartmap)
                    {
                        table.Rows.Add(item.EntityGroupID, item.BusinessUnitID, item.DivisionID, item.DeptID,item.DivisionID);
                    }
                    var parameters = new DynamicParameters();
                    parameters.Add("@tblDepartmentDetail", table.AsTableValuedParameter("dbo.utt_DepartmentDetails")); // Ensure this matches the table type
                    parameters.Add("@Mode", "ADD");
                    parameters.Add("@IsChild", addDept.IsChild);
                    parameters.Add("@DeptCode", addDept.DeptCode);
                    parameters.Add("@DeptDesc", addDept.DeptDesc);
                    parameters.Add("@ParentID", addDept.ParentID);
                    //parameters.Add("@ColourCode", addDept.ColourCode);
                    //parameters.Add("@TimeZoneID", addDept.TimeZoneID);
                    parameters.Add("@CreatedBy", struserGuid);
                    parameters.Add("@Msg", dbType: DbType.String, direction: ParameterDirection.Output, size: 2000);             
                    var result = await connection.ExecuteAsync(
                        "sp_Department",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    string message = parameters.Get<string>("@Msg");

                    return message; // Ensure this matches the controller's expectation
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "0";
            }
        }

        public async Task<string> UpdateDepartmentDetails(EditDept EditDept)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var table = new DataTable();
                    table.Columns.Add("EntityGroupID", typeof(long));
                    table.Columns.Add("BusinessUnitID", typeof(long));
                    table.Columns.Add("CostCenterID", typeof(long));
                    table.Columns.Add("DeptID", typeof(long));
                    table.Columns.Add("DivisionID", typeof(long));
                    foreach (var item in EditDept.lstDepartmap)
                    {
                        table.Rows.Add(item.EntityGroupID, item.BusinessUnitID, item.DivisionID, item.DeptID,item.DivisionID);
                    }

                    var parameters = new DynamicParameters();
                    parameters.Add("@tblDepartmentDetail", table.AsTableValuedParameter("dbo.utt_DepartmentDetails")); // Ensure this matches the table type
                    parameters.Add("@Mode", Common.PageMode.EDIT);
                    parameters.Add("@IsChild", EditDept.IsChild);
                    parameters.Add("@DeptCode", EditDept.DeptCode);
                    parameters.Add("@DeptDesc", EditDept.DeptDesc);
                    parameters.Add("@ParentID", EditDept.ParentID);
                    parameters.Add("@ParentID", EditDept.ParentID);
                    parameters.Add("@DeptGuid", EditDept.DeptGuid);
                    parameters.Add("@Msg", dbType: DbType.String, direction: ParameterDirection.Output, size: 2000);

                    var result = await connection.ExecuteAsync(
                        "sp_Department",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    string message = parameters.Get<string>("@Msg");

                    return message;// Ensure this matches the controller's expectation
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "0";
            }
        }


        public async Task<List<DeptDeleteResult>> DeleteDepartmentDetails(List<DeleteDeptList> lstDeleteDept)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var table = new DataTable();
                    table.Columns.Add("GUID", typeof(Guid));
                  
                    foreach (var item in lstDeleteDept)
                    {
                        table.Rows.Add(item.Guid ?? Guid.Empty); // or handle null however your schema expects
                    }

                    var parameters = new DynamicParameters();
                    parameters.Add("@tblDeleteByGUID", table.AsTableValuedParameter("dbo.utt_DeleteByGUID"));
                    parameters.Add("@Mode", "DELETE");
                    parameters.Add("@Msg", dbType: DbType.String, direction: ParameterDirection.Output, size: 2000);

                    var result = await connection.QueryAsync<DeptDeleteResult>(
                        "sp_Department",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    string message = parameters.Get<string>("@Msg");

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<DeptDeleteResult>(); // Return empty list on failure instead of string
            }
        }


    }
}
