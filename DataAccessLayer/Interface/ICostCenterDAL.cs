using DataAccessLayer.Model;

using System.Data;

namespace DataAccessLayer.Interface
{
    public interface ICostCenterDAL
    {
        Task<IEnumerable<GetCostCenterModel>> GetAllCostCenter();
        Task<(GetCostCenterModel getCostCenterModel, List<DropDownModel> getBusinessEntityTables, List<MultiSelectionDropDownModel>? getDivisionDatatables, List<MultiSelectionDropDownModel?> getDepartmentDatatables)> GetCostCenterByGuId(string GuId);
        Task<string> AddCostCenterAsync(CostCenterModel model, DataTable dataTable);
        Task<string> UpdateCostCenterAsync(UpdateCostCenterModel model, DataTable dataTable);
        Task<string> DeleteCostCenterAsync(DataTable deleteLevelDetailTable);
        Task<IEnumerable<DropDownModel>> getMapParentCostCenter();
        Task<IEnumerable<DropDownModel>> GetMapBusinessUnit();
        Task<IEnumerable<DropDownModel>> GetMapDivisionCost();
        Task<IEnumerable<DropDownModel>> GetMapDepartment();

    }
}
