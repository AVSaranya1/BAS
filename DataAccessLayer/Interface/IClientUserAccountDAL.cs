using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IClientUserAccountDAL
    {
        Task<List<GetClientUserAccountModel?>> GetAllUserAccount(long UpdatedBy, string LevelDetailGuid);

        Task<List<ClientUserPolicyName?>> getAllUserPolicyInDropdown(string ClientDBName);

        Task<List<ClientUserLanguageName?>> getAllUserLanguageInDropdown();
        Task<List<ClientUserTimeZoneName?>> getAllUserTimeZoneInDropdown();
        Task<(List<ClientUserAccountModel?> InsertedUsers, long? RetVal, string? Msg)> InsertCheckUserAccount(string UserName);
        Task<(List<ClientUserAccountModel?> InsertedUsers, List<OrgDetails?> OrgDetails, long? RetVal, string? Msg)> InsertUpdateUserAccount(ClientUserAccountModel? UM);

       
        Task<(List<ResetPassword?> PasswordReset, int? RetVal, string? Msg)> ResetPasswordInUserAccount(ResetPassword PasswordReset);

        Task<(bool deleteuseraccount, List<DeleteResult> deleteResults)> DeleteUserAccount(long UpdatedBy, ClientDeleteUserAccount deleteUserAccount);

        //Task<(GetClientUserAccount? userAccounts, List<ClientGetUserAccountRole>? UserRoles, List<ClientGetUserAccountModules>? Modules)> GetUserAccountByGUId(string? GUId, long UpdatedBy);
        Task<(GetClientUserAccount? userAccounts, List<ClientGetUserAccountRole>? UserRoles)> GetUserAccountByGUId(string? GUId, long UpdatedBy);

        Task<List<GetClientRoleName?>> getAllUserRoleInDropdown(string ClientDBName);

        Task<(List<UpdateClientUserAccountModel?> updateuseraccount, List<OrgDetails?> OrgDetails, long? RetVal, string? Msg)> UpdateUserAccountAsync(UpdateClientUserAccountModel? userAccount);

        Task<(List<ClientUnlockUser?> unlockuser, int? RetVal, string? Msg)> UnlockUserAsync(ClientUnlockUser? model);
        
    }
}
