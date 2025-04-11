using DataAccessLayer.Model;
using DataAccessLayer.Model.BusinessEntity;
using System.Data;

namespace DataAccessLayer.Interface;

public interface IBusinessEntityDAL
{
    Task<IEnumerable<BusinessEntityModel>> GetAllBusinessEntity();
    Task<BusinessEntityModel> GetBusinessEntityById(long Id);    
    Task<string> AddBusinessEntityAsync(AddBusinessEntityModel model);
    Task<string> UpdateBusinessEntityAsync(UpdateBusinessEntityModel model);
    Task<string> DeleteBusinessEntityAsync(DataTable deleteLevelDetailTable);
    Task<IEnumerable<DropDownModel>> GetMapParentBusinessUnit();
    Task<IEnumerable<DropDownModel>> GetMapEntityGroup();
}
