using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Management.XEvent;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data.Common;
using System.Transactions;


namespace DataAccessLayer.Implementation
{
    public class LoginDAL : BaseRepository, ILoginDAL
    {
        public LoginDAL(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        { }

        public async Task<List<ResultModel>> UserLogin(LoginModel objLoginModel)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Mode", "Get");
            dynamicParameters.Add("@UserName", objLoginModel.UserName);
            dynamicParameters.Add("@Password", objLoginModel.Password);
            dynamicParameters.Add("@UserType", objLoginModel.UserType);
            dynamicParameters.Add("@IPAddress", objLoginModel.IPAddress);
            dynamicParameters.Add("@DeviceName", objLoginModel.DeviceName);
            dynamicParameters.Add("@BrowserName", objLoginModel.BrowserName);

            using var multi = await Connection.QueryMultipleAsync(
                "sp_Authentication",
                dynamicParameters,
                commandType: CommandType.StoredProcedure);

            var res = multi.Read<ResultModel>().ToList();

            if (res is { Count: > 0 })
            {
                switch (res[0].RetVal)
                {
                    case -1:
                        return res;

                    case 1:
                        var details = multi.Read<LoginDetailModel>().ToList();
                        res[0].lstLoginDetails = details;
                        break;
                }
            }
            return res;
        }

        public async Task<List<ResultModel>> ClientUserLogin(LoginModel objLoginModel)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Mode", "Get");
            dynamicParameters.Add("@UserName", objLoginModel.UserName);
            dynamicParameters.Add("@Password", objLoginModel.Password);
            dynamicParameters.Add("@UserType", objLoginModel.UserType);
            dynamicParameters.Add("@IPAddress", objLoginModel.IPAddress);
            dynamicParameters.Add("@DeviceName", objLoginModel.DeviceName);
            dynamicParameters.Add("@BrowserName", objLoginModel.BrowserName);

            using var multi = await Connection.QueryMultipleAsync(
                "sp_Authentication",
                dynamicParameters,
                commandType: CommandType.StoredProcedure);

            var res = multi.Read<ResultModel>().ToList();

            if (res.Any() && res[0].RetVal == 1)
            {
                var details = multi.Read<LoginDetailModel>().ToList();
                res[0].lstLoginDetails = details;
            }
            return res;
        }

        public async Task<List<ResultModel>> GetUserID(LoginModel objloginModel)
        {
            DynamicParameters dyParameter = new DynamicParameters();
            dyParameter.Add("@Mode", Common.PageMode.GET_USER_ID);
            dyParameter.Add("@UserName", objloginModel.UserName);
            dyParameter.Add("@Password", objloginModel.Password);
            dyParameter.Add("@UserType", objloginModel.UserType);
            dyParameter.Add("@IPAddress", objloginModel.IPAddress);
            dyParameter.Add("@DeviceName", objloginModel.DeviceName);
            dyParameter.Add("@BrowserName", objloginModel.BrowserName);

            var multi = await Connection.QueryMultipleAsync("sp_Authentication",
                dyParameter,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<ResultModel>().ToList();

            if (res is { Count: > 0 })
            {
                switch (res[0].RetVal)
                {
                    case -1:
                        return res;

                    case 1:
                        var details = multi.Read<LoginDetailModel>().ToList();
                        res[0].lstLoginDetails = details;
                        break;
                }
            }
            return res;
        }

        public async Task<List<OrganisationDBDetails>> GetOrganisationWithDBDetails(LoginModel objloginModel)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Mode", Common.PageMode.GET_ORG);
            dynamicParameters.Add("@UserGUID", objloginModel.Guid);

            var multi = await Connection.QueryMultipleAsync("sp_Authentication",
                dynamicParameters,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<OrganisationDBDetails>().ToList();
            return res;
        }

        //public async Task<List<GetDropDownDataModel>> GetDDlLanguage(string Mode, string RefID1, string RefID2, string RefID3)
        //{
        //    DynamicParameters dynamicParameters = new DynamicParameters();
        //    dynamicParameters.Add("@Mode", Mode);
        //    dynamicParameters.Add("RefID1", RefID1);
        //    dynamicParameters.Add("RefID2", RefID2);
        //    dynamicParameters.Add("RefID3", RefID3);

        //    var multi = await Connection.QueryMultipleAsync("sp_ListData",
        //        dynamicParameters,
        //        commandType: CommandType.StoredProcedure);
        //    var res = multi.Read<GetDropDownDataModel>().ToList();
        //    return res;
        //}

        //public async Task<List<GetDropDownDataModel>> GetDDlModule(string Mode, string RefID1, string RefID2, string RefID3)
        //{
        //    DynamicParameters dynamicParameters = new DynamicParameters();
        //    dynamicParameters.Add("@Mode", Mode);
        //    dynamicParameters.Add("RefID1", RefID1);
        //    dynamicParameters.Add("RefID2", RefID2);
        //    dynamicParameters.Add("RefID3", RefID3);

        //    var multi = await Connection.QueryMultipleAsync("sp_ListData",
        //        dynamicParameters,
        //        commandType: CommandType.StoredProcedure);
        //    var res = multi.Read<GetDropDownDataModel>().ToList();
        //    return res;
        //}

        public async Task<bool> UpdateLoginDetails(LoginDetails loginDetails)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Mode", loginDetails.Mode);
            dynamicParameters.Add("@UserId", loginDetails.UserId);
            dynamicParameters.Add("@UserGuid", loginDetails.UserGuid);
            dynamicParameters.Add("@Token", loginDetails.Token);
            dynamicParameters.Add("@IPAddress", loginDetails.IPAddress);
            dynamicParameters.Add("@DeviceInfo", loginDetails.DeviceInfo);
            dynamicParameters.Add("@DeviceInfo", loginDetails.DeviceInfo);

            var multi = await Connection.QueryMultipleAsync("sp_LoginDetails",
                dynamicParameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var result = await multi.ReadFirstOrDefaultAsync<int>();
            return result > 0 ? true : false;
        }

        public async Task<bool> InsertLoginDetails(LoginDetails loginDetails)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Mode", loginDetails.Mode);
                dynamicParameters.Add("@UserId", loginDetails.UserId);
                dynamicParameters.Add("@UserGuid", loginDetails.UserGuid);
                dynamicParameters.Add("@Token", loginDetails.Token);
                dynamicParameters.Add("@IPAddress", loginDetails.IPAddress);
                dynamicParameters.Add("@DeviceInfo", loginDetails.DeviceInfo);
                // Adding Output Parameters
                dynamicParameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                dynamicParameters.Add("@Msg", dbType: DbType.String, size: 2000, direction: ParameterDirection.Output);

                // Execute stored procedure using existing transaction
                await Connection.ExecuteAsync(
                   "sp_LoginDetails",
                dynamicParameters,
                   transaction: Transaction,
                   commandType: CommandType.StoredProcedure
               );

                int result = dynamicParameters.Get<int>("@RetVal");
                string message = dynamicParameters.Get<string>("@Msg");

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
