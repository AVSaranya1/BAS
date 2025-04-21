using DataAccessLayer.Interface;

namespace DataAccessLayer.Uow.Interface
{
    public interface IUowDivision : IDisposable
    {
        IDivisionDAL ClientDivisionDALRepo { get; }
        void Commit();
    }
}
