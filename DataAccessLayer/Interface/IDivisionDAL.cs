using DataAccessLayer.Model;

namespace DataAccessLayer.Interface
{
    public interface IDivisionDAL : IDisposable
    {
        Task<List<GetDivisionModel>> GetAllClientDivision(GetDivisionModel getClientDivisionModel);
        Task<(bool InsertClientDivision, long RetVal, string Msg)> InsertUpdateClientDivision(DivisionModel NM);
        Task<(bool UpdateClientDivision, long RetVal, string Msg)> UpdateClientDivision(UpdateDivision NM);
        Task<(bool deleteClientDivision, List<DeleteDivisionResult> deleteResults)> DeleteClientDivision(long Id, DeleteDivision deleteClientDivision);
        Task<(GetDivisionModel? getClientDivisionModel,  List<DropDownModel?> getBusinessEntityDatatables, List<DropDownModel?> getCostCenterDivisionDatatables, List<MultiSelectionDropDownModel?> getDepartmentDatatables)> GetClientDivisionByGUId(string GuId);
        Task<(List<DropDownModel>? getBusinessEntityDatatables, List<DropDownModel?> getCostCenterDivisionDatatables, List<DropDownModel?> getDepartmentDatatables)> GetClientDivisionDeptCatMap();
        Task <List<DropDownModel>> getParentDivisionMap();
    }
    
}
