using DataAccessLayer.Model;

using System.Data;

namespace DataAccessLayer.Interface
{
    public interface ICostCenterDAL
    {
        Task<IEnumerable<GetCostCenterModel>> GetAllCostCenter();
        Task<(GetCostCenterModel getCostCenterModel, List<MultiSelectionDropDownModel> getBusinessEntityTables, List<MultiSelectionDropDownModel>? getDivisionDatatables, List<MultiSelectionDropDownModel?> getDepartmentDatatables)> GetCostCenterByGuId(string GuId);
        Task<string> AddCostCenterAsync(CostCenterModel model, DataTable dataTable);
        Task<string> UpdateCostCenterAsync(UpdateCostCenterModel model, DataTable dataTable);
        Task<string> DeleteCostCenterAsync(DataTable deleteLevelDetailTable, string? ModifiedBy);
        Task<IEnumerable<DropDownModel>> getMapParentCostCenter();
        Task<(List<DropDownModel?> getBusinessEntityTables, List<DropDownModel?> getDivisionDatatables, List<DropDownModel?> getDepartmentDatatables)> getMapBUDivisionDept();


    }
}
