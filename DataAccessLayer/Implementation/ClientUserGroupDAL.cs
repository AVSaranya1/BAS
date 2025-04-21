using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class ClientUserGroupDAL : RepositoryBase, IClientUserGroupDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClientUserGroupDAL(IDbTransaction _transaction) : base(_transaction)
        {
            
        }
        public async Task<(bool deleteuserGroup, List<DeleteClientUserGroupResult> deleteResults)> DeleteUserPolicy(long UpdatedBy, DeleteClientUserGroup deleteUserPolicy)
        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblDelete", JsonConvert.SerializeObject(deleteUserPolicy.UserGroupDeleteTable));
            parameters.Add("@UpdatedBy", UpdatedBy);
            parameters.Add("@Mode", Common.PageMode.DELETE);
            var Result = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            List<DeleteClientUserGroupResult> DeleteUserAccount = (await Result.ReadAsync<DeleteClientUserGroupResult>()).ToList();
            while (!Result.IsConsumed)
            {
                await Result.ReadAsync();
            }

            bool res = DeleteUserAccount.Any();



            return (res, DeleteUserAccount.ToList());

        }
        public async Task<List<GetClientUserGroupModel?>> GetAllUserPolicy(long UpdatedBy, string ClientDBName)
        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupGUID", string.Empty);
            parameters.Add("@UpdatedBy", UpdatedBy);
            parameters.Add("@Mode", Common.PageMode.GET);
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<GetClientUserGroupModel?>().ToList();
        }

        public async Task<GetClientUserGroupModel?> GetUserPolicyByGUId(string? GUId, string? ClientDBName)
        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupGUID", GUId);
            parameters.Add("@Mode", Common.PageMode.GET);
            
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<GetClientUserGroupModel?>().First();

            return res;

        }

        public async Task<(bool InsertUserGroup, int RetVal, string Msg)> InsertUpdateUserPolicy(ClientUserGroupModel? model)
        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@MinPasswordLength", model?.MinPasswordLength);
            parameters.Add("@MaxPasswordLength", model?.MaxPasswordLength);
            parameters.Add("@MustContainUppercase", model?.MustContainUppercase);
            parameters.Add("@MustContainLowercase", model?.MustContainLowercase);
            parameters.Add("@MustContainDigit", model?.MustContainDigit);
            parameters.Add("@MustContainSpecialCharacter", model?.MustContainSpecialCharacter);
            parameters.Add("@UserGroupID", model?.UserGroupID);
            parameters.Add("@UserGroup", model?.UserGroupCode);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@RestrictFailedLogin", model?.RestrictFailedLogin);
            parameters.Add("@FailedLoginCount", model?.FailedLoginCount);
            parameters.Add("@PasswordExpiry", model?.PasswordExpiry);
            parameters.Add("@PasswordExpiryDays", model?.PasswordExpiryDays);
            parameters.Add("@PasswordExpiryAlertDays", model?.PasswordExpiryAlertDays);
            parameters.Add("@RestrictPasswordReuse", model?.RestrictPasswordReuse);
            parameters.Add("@PasswordCount", model?.PreviousPasswordCannotReuse);
            parameters.Add("@2FAAuthentication", model?.twoFAAuthentication);
            parameters.Add("@Enforce2FA", model?.Enforce2FA);
            parameters.Add("@RetentionPolicy", model?.RetentionPolicy);
            parameters.Add("@RetentionPolicyDays", model?.RetentionDuration);
            parameters.Add("@BlockAccessonceAllAttemptConsumed", model?.BlockAccessOnceAllAttemptConsumed);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@Mode", Common.PageMode.ADD);
            parameters.Add("@Guid", Guid.NewGuid());
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var UserGroup = (await multi.ReadAsync<ClientUserGroupModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = UserGroup.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);

        }

        public async Task<(bool UpdateUserGroup, int RetVal, string Msg)> UpdateUserPolicyAsync(ClientUpdateUserGroupModel? model)
        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", model?.UserGroupID);
            parameters.Add("@MinPasswordLength", model?.MinPasswordLength);
            parameters.Add("@MaxPasswordLength", model?.MaxPasswordLength);
            parameters.Add("@MustContainUppercase", model?.MustContainUppercase);
            parameters.Add("@MustContainLowercase", model?.MustContainLowercase);
            parameters.Add("@MustContainDigit", model?.MustContainDigit);
            parameters.Add("@MustContainSpecialCharacter", model?.MustContainSpecialCharacter);
            parameters.Add("@UserGroup", model?.UserGroupCode);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@RestrictFailedLogin", model?.RestrictFailedLogin);
            parameters.Add("@FailedLoginCount", model?.FailedLoginCount);
            parameters.Add("@PasswordExpiry", model?.PasswordExpiry);
            parameters.Add("@PasswordExpiryDays", model?.PasswordExpiryDays);
            parameters.Add("@RestrictPasswordReuse", model?.RestrictPasswordReuse);
            parameters.Add("@PasswordCount", model?.PreviousPasswordCannotReuse);
            parameters.Add("@RetentionPolicy", model?.RetentionPolicy);
            parameters.Add("@RetentionPolicyDays", model?.RetentionDuration);
            parameters.Add("@2FAAuthentication", model?.twoFAAuthentication);
            parameters.Add("@Enforce2FA", model?.Enforce2FA);
            parameters.Add("@BlockAccessonceAllAttemptConsumed", model?.BlockAccessOnceAllAttemptConsumed);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@UserGroupGUID", model?.UserPolicyGuid);
            parameters.Add("@Mode", Common.PageMode.EDIT);
            //Output parameters
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var UserGroup = (await multi.ReadAsync<UserGroupModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = UserGroup.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);
        }
    }
}
