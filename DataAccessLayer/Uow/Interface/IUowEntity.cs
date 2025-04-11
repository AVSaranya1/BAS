using DataAccessLayer.Interface;

namespace DataAccessLayer.Uow.Interface;

public interface IUowEntity: IDisposable
{
    IBusinessEntityDAL EntityDALRepo { get; }
    void Commit();
}
