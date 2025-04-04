using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Uow.Interface
{
    public interface IUowClientUser : IDisposable
    {
        IClientUserAccountDAL UserAccountDALRepo { get; }
        void Commit();
    }
}
