using DataAccessLayer.Interface;

namespace DataAccessLayer.Uow.Interface;
public interface IUowDropdown:IDisposable
{    
        IDropdownDAL MasterDALRepo { get; }
        void Commit();
}

