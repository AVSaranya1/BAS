using DataAccessLayer.Model;
using System.Data;

namespace DataAccessLayer.Interface;

public interface IEntityDAL
{
    Task<List<EntityModel>> GetAllLevelDetail();
    Task<EntityModel> GetLevelDetailByGuid(string Guid);    
    Task<bool> InsertLevelDetailAsync(EntityModel model);
    Task<bool> UpdateLevelDetailAsync(EntityModel model);
    Task<(bool deleteLevelDetail, List<DeleteLevelDetailResult> deleteResults)> DeleteLevelDetailAsync(DataTable deleteLevelDetailTable);
}
