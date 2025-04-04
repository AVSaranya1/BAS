using DataAccessLayer.Implementation;
using DataAccessLayer.Services;

namespace DataAccessLayer.Interface
{
    public interface IGUIDDAL
    {
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUID(string? UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetClientGUID(string? UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnUserAccountRoleGuid(string UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnRoleGuid(string UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnOrgName(string UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnUserPolicy(string UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnClientUserPolicy(string? UserGuid,string? ClientDBName);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnNationality(string UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnMailServer(string UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnClientRoleGuid(string UserGuid, string? ClientDBName);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnClientLevelInfo(string UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnClientDivision(string? updatedGuidBy);
    }
}
