using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IEntityGroupDAL
    {
      Task<List<EntityGroupModel>> GetEntityGroup(EntityGroupModel entityGroupModel);
      Task<List<EntityGroupModel>> GetEntityGroupDetails(EntityGroupModel entityGroupModel);

        Task<string> AddEntityGroup(EntityGroupModel entityGroupModel);
        Task<string> EditEntityGroup(EntityGroupModel entityGroupModel);

        Task<List<DeleteResultModel>> DeleteEntityGroup(List<EntityGroupDel> lstEntityGroupDel, string strMode,string strUserGuid);
    }
}
