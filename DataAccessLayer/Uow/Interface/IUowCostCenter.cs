using DataAccessLayer.Interface;

namespace DataAccessLayer.Uow.Interface
{
    public interface IUowCostCenter : IDisposable
    {
        ICostCenterDAL CostCenterDALRepo { get; }
        void Commit();
    }
}
