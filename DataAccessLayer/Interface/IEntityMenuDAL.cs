using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IEntityMenuDAL
    {
        Task<List<EntityMenuModel>> GetEntityMenu(string? strMode,string? struserGuid);
        Task<List<MenusModel>> GetMenu(string UserNameGuid);
    }
}
