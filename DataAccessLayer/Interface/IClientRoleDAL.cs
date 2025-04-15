using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IClientRoleDAL
    {
        Task<List<GetClientRoleModel>> GetAllRole(long UpdatedBy, string ClientDBName);

        Task<(List<ClientRoleModel?> roleModels, long? RetVal, string? Msg)> InsertUpdateRole(ClientRoleModel? LM);
        Task<(List<GetClientRoleModel?> roleModels, long? RetVal, string? Msg)> UpdateRole(GetClientRoleModel? LM);
        Task<(bool? DeleteRole, List<DeleteClientRoleInformation?> deleteRoleInformation)> DeleteRole(ClientRolesDelete? rolesDelete, long UserId);
        Task<(ClientRoleModel? rolemodel, List<ClientModules?> ModuleDatatable)> getModulesBasedOnRole(string? RoleGUID, long? UserGUID,string? ClientDBName);
        Task<(List<GetClientRoleModel?> roleModels, long? RetVal, string? Msg)> EditUpdateRoleAsync(GetClientRoleModel roleModel);
    }
}
