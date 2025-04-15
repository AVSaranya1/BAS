using DataAccessLayer.Model;

namespace DataAccessLayer.Interface
{
    public interface IClientUserGroupDAL
    {
        Task<List<GetClientUserGroupModel?>> GetAllUserPolicy(long UpdatedBy, string ClientDBName);
        Task<(bool InsertUserGroup, int RetVal, string Msg)> InsertUpdateUserPolicy(ClientUserGroupModel? UM);

        Task<(bool deleteuserGroup, List<DeleteClientUserGroupResult> deleteResults)> DeleteUserPolicy(long UpdatedBy, DeleteClientUserGroup deleteUserGroup);
        Task<GetClientUserGroupModel?> GetUserPolicyByGUId(string? GUId,string? ClientDBName);
        Task<(bool UpdateUserGroup, int RetVal, string Msg)> UpdateUserPolicyAsync(ClientUpdateUserGroupModel? userAccount);
    }
}
