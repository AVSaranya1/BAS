using DataAccessLayer.Interface;

namespace DataAccessLayer.Uow.Interface;

public interface IUowEntity: IDisposable
{
    IEntityDAL EntityDALRepo { get; }
    void Commit();
}
