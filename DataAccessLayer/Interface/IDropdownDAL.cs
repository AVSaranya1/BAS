using DataAccessLayer.Model;
namespace DataAccessLayer.Interface;
public interface IDropdownDAL
{
    Task<IEnumerable<DropDownModel>> getLevel(long userID);
    Task<IEnumerable<DropDownModel>> getParentEntity(long UserID,string RefID1,string RefID2);
    Task<IEnumerable<DropDownModel>> getCountry();
    Task<IEnumerable<DropDownModel>> getCurrency();
    Task<IEnumerable<TimezoneModel>> getTimeZone(long? RefID1);
    Task<IEnumerable<DropDownModel>> getEntityGroup();
}
